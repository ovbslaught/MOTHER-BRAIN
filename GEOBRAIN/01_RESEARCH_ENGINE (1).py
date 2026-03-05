#!/usr/bin/env python3
"""
GEOLOGOS RESEARCH ENGINE: Production Ready
Web scraper + multi-source aggregation + citation database + mesh network
Deploy: python research_engine.py
"""

import asyncio
import json
import hashlib
import uuid
from datetime import datetime
from typing import List, Dict, Any, Optional
from dataclasses import dataclass, asdict
from enum import Enum
import re
from urllib.parse import urlparse

import aiohttp
from bs4 import BeautifulSoup
import redis
import psycopg2
from psycopg2.extras import RealDictCursor
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

# ============================================================================
# CONFIGURATION
# ============================================================================

DATABASE_URL = "postgresql://geologos_app:secure_password_change_me@localhost:5432/geologos"
REDIS_URL = "redis://localhost:6379"
RESEARCH_DB = "geologos_research"

# ============================================================================
# ENUMS & DATA STRUCTURES
# ============================================================================

class CitationFormat(Enum):
    APA = "apa"
    MLA = "mla"
    CHICAGO = "chicago"
    HARVARD = "harvard"
    IEEE = "ieee"
    BIBTEX = "bibtex"

@dataclass
class Source:
    """Research source"""
    id: str
    url: str
    title: str
    authors: List[str]
    published_date: Optional[str]
    accessed_date: str
    content: str
    content_hash: str
    domain: str
    source_type: str  # article, paper, book, website, etc
    credibility_score: float
    tags: List[str]
    created_at: str

@dataclass
class Citation:
    """Generated citation"""
    source_id: str
    format: CitationFormat
    text: str
    bibtex: str
    in_text: str
    created_at: str

@dataclass
class ResearchQuery:
    """Research query with results"""
    query: str
    sources: List[Source]
    total_results: int
    search_time_ms: float
    mesh_results: List[str]  # From P2P mesh peers

# ============================================================================
# WEB SCRAPER
# ============================================================================

class WebScraper:
    """Scrapes and extracts structured data from URLs"""
    
    def __init__(self):
        self.session = None
        self.headers = {
            'User-Agent': 'GEOLOGOS-Research-Engine/1.0 (+http://geologos.local)'
        }
    
    async def init_session(self):
        """Initialize async session"""
        self.session = aiohttp.ClientSession(headers=self.headers)
    
    async def close_session(self):
        """Close async session"""
        if self.session:
            await self.session.close()
    
    async def scrape_url(self, url: str) -> Dict[str, Any]:
        """Scrape URL and extract metadata + content"""
        try:
            async with self.session.get(url, timeout=aiohttp.ClientTimeout(total=10)) as response:
                if response.status != 200:
                    return {"error": f"HTTP {response.status}"}
                
                html = await response.text()
                soup = BeautifulSoup(html, 'html.parser')
                
                # Extract metadata
                title = self._extract_title(soup)
                authors = self._extract_authors(soup)
                published_date = self._extract_published_date(soup)
                content = self._extract_content(soup)
                
                return {
                    "url": url,
                    "title": title,
                    "authors": authors,
                    "published_date": published_date,
                    "content": content,
                    "domain": urlparse(url).netloc,
                    "scraped_at": datetime.utcnow().isoformat()
                }
        except Exception as e:
            return {"error": str(e)}
    
    def _extract_title(self, soup: BeautifulSoup) -> str:
        """Extract page title"""
        og_title = soup.find("meta", property="og:title")
        if og_title:
            return og_title.get("content", "")
        
        title_tag = soup.find("title")
        if title_tag:
            return title_tag.string or ""
        
        h1 = soup.find("h1")
        if h1:
            return h1.get_text().strip()
        
        return ""
    
    def _extract_authors(self, soup: BeautifulSoup) -> List[str]:
        """Extract author information"""
        authors = []
        
        # Try meta author
        author_meta = soup.find("meta", {"name": "author"})
        if author_meta:
            authors.append(author_meta.get("content", ""))
        
        # Try article author schema
        article_author = soup.find("span", {"class": ["author", "by-author"]})
        if article_author:
            authors.append(article_author.get_text().strip())
        
        return [a for a in authors if a]
    
    def _extract_published_date(self, soup: BeautifulSoup) -> Optional[str]:
        """Extract publication date"""
        # Try various date meta tags
        for attr in ["publish_date", "published_time", "datePublished"]:
            date_meta = soup.find("meta", {"property": f"article:{attr}"})
            if date_meta:
                return date_meta.get("content")
        
        # Try schema.org datePublished
        date_meta = soup.find("meta", {"itemprop": "datePublished"})
        if date_meta:
            return date_meta.get("content")
        
        return None
    
    def _extract_content(self, soup: BeautifulSoup) -> str:
        """Extract main content"""
        # Remove script and style elements
        for script in soup(["script", "style"]):
            script.decompose()
        
        # Try article content
        article = soup.find("article")
        if article:
            return article.get_text(separator=" ", strip=True)[:5000]
        
        # Try main content area
        main = soup.find(["main", "div"], {"class": ["main-content", "content", "post-content"]})
        if main:
            return main.get_text(separator=" ", strip=True)[:5000]
        
        # Fallback to body
        body = soup.find("body")
        if body:
            return body.get_text(separator=" ", strip=True)[:5000]
        
        return ""

# ============================================================================
# CITATION GENERATOR
# ============================================================================

class CitationGenerator:
    """Generate citations in multiple formats"""
    
    @staticmethod
    def generate(source: Source, format: CitationFormat) -> Citation:
        """Generate citation in requested format"""
        
        if format == CitationFormat.APA:
            text = CitationGenerator._apa(source)
        elif format == CitationFormat.MLA:
            text = CitationGenerator._mla(source)
        elif format == CitationFormat.CHICAGO:
            text = CitationGenerator._chicago(source)
        elif format == CitationFormat.HARVARD:
            text = CitationGenerator._harvard(source)
        elif format == CitationFormat.IEEE:
            text = CitationGenerator._ieee(source)
        elif format == CitationFormat.BIBTEX:
            text = CitationGenerator._bibtex(source)
        else:
            text = CitationGenerator._apa(source)
        
        bibtex = CitationGenerator._bibtex(source)
        in_text = CitationGenerator._in_text(source)
        
        return Citation(
            source_id=source.id,
            format=format,
            text=text,
            bibtex=bibtex,
            in_text=in_text,
            created_at=datetime.utcnow().isoformat()
        )
    
    @staticmethod
    def _apa(source: Source) -> str:
        """APA format"""
        authors = ", ".join(source.authors) if source.authors else "Unknown"
        date = source.published_date.split("T")[0] if source.published_date else "n.d."
        return f"{authors} ({date}). {source.title}. Retrieved from {source.url}"
    
    @staticmethod
    def _mla(source: Source) -> str:
        """MLA format"""
        authors = ", ".join(source.authors) if source.authors else "Unknown"
        date = source.published_date.split("T")[0] if source.published_date else "n.d."
        return f"{authors}. \"{source.title}.\" Web. {date}. {source.url}"
    
    @staticmethod
    def _chicago(source: Source) -> str:
        """Chicago format"""
        authors = ", ".join(source.authors) if source.authors else "Unknown"
        date = source.published_date.split("T")[0] if source.published_date else "n.d."
        return f"{authors}. \"{source.title}.\" Accessed {date}. {source.url}."
    
    @staticmethod
    def _harvard(source: Source) -> str:
        """Harvard format"""
        authors = ", ".join(source.authors) if source.authors else "Unknown"
        date = source.published_date.split("T")[0] if source.published_date else "n.d."
        return f"{authors}, {date}. {source.title}. Available at: {source.url}"
    
    @staticmethod
    def _ieee(source: Source) -> str:
        """IEEE format"""
        authors = ", ".join(source.authors) if source.authors else "Unknown"
        date = source.published_date.split("T")[0] if source.published_date else "n.d."
        return f"[Online] {authors}. {source.title}. Available: {source.url}. [Accessed: {date}]."
    
    @staticmethod
    def _bibtex(source: Source) -> str:
        """BibTeX format"""
        key = source.id
        authors = " and ".join(source.authors) if source.authors else "Unknown"
        date = source.published_date.split("T")[0] if source.published_date else "n.d."
        return f"""@misc{{{key},
  author = {{{authors}}},
  title = {{{source.title}}},
  url = {{{source.url}}},
  year = {{{date.split('-')[0] if date != 'n.d.' else 'n.d.'}}},
  urldate = {{{datetime.utcnow().isoformat().split('T')[0]}}}
}}"""
    
    @staticmethod
    def _in_text(source: Source) -> str:
        """In-text citation"""
        authors = source.authors[0] if source.authors else "Unknown"
        year = source.published_date.split("-")[0] if source.published_date else "n.d."
        return f"({authors}, {year})"

# ============================================================================
# RESEARCH ENGINE (DATABASE LAYER)
# ============================================================================

class ResearchDatabase:
    """PostgreSQL backend for research sources"""
    
    def __init__(self, db_url: str):
        self.db_url = db_url
        self.conn = None
    
    def connect(self):
        """Connect to database"""
        self.conn = psycopg2.connect(self.db_url)
        self._create_tables()
    
    def _create_tables(self):
        """Create research tables"""
        cursor = self.conn.cursor()
        
        # Sources table
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS research_sources (
                id VARCHAR(36) PRIMARY KEY,
                url TEXT UNIQUE NOT NULL,
                title VARCHAR(500),
                authors TEXT[],
                published_date TIMESTAMP,
                accessed_date TIMESTAMP NOT NULL,
                content TEXT,
                content_hash VARCHAR(64),
                domain VARCHAR(255),
                source_type VARCHAR(50),
                credibility_score FLOAT DEFAULT 0.5,
                tags TEXT[],
                created_at TIMESTAMP NOT NULL,
                updated_at TIMESTAMP NOT NULL
            )
        """)
        
        # Citations table
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS research_citations (
                id VARCHAR(36) PRIMARY KEY,
                source_id VARCHAR(36) NOT NULL REFERENCES research_sources(id),
                format VARCHAR(20) NOT NULL,
                text TEXT NOT NULL,
                bibtex TEXT,
                in_text TEXT,
                created_at TIMESTAMP NOT NULL
            )
        """)
        
        # Search queries table (for analytics)
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS research_queries (
                id VARCHAR(36) PRIMARY KEY,
                query TEXT NOT NULL,
                results_count INT,
                search_time_ms INT,
                created_at TIMESTAMP NOT NULL
            )
        """)
        
        self.conn.commit()
        cursor.close()
    
    def add_source(self, source: Source) -> bool:
        """Add source to database"""
        try:
            cursor = self.conn.cursor()
            cursor.execute("""
                INSERT INTO research_sources 
                (id, url, title, authors, published_date, accessed_date, content, 
                 content_hash, domain, source_type, credibility_score, tags, created_at, updated_at)
                VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
            """, (
                source.id, source.url, source.title, source.authors,
                source.published_date, source.accessed_date, source.content,
                source.content_hash, source.domain, source.source_type,
                source.credibility_score, source.tags,
                source.created_at, source.created_at
            ))
            self.conn.commit()
            cursor.close()
            return True
        except Exception as e:
            print(f"Error adding source: {e}")
            return False
    
    def search_sources(self, query: str, limit: int = 20) -> List[Source]:
        """Search sources by keyword"""
        try:
            cursor = self.conn.cursor(cursor_factory=RealDictCursor)
            cursor.execute("""
                SELECT * FROM research_sources
                WHERE title ILIKE %s OR content ILIKE %s OR tags @> %s
                ORDER BY credibility_score DESC
                LIMIT %s
            """, (f"%{query}%", f"%{query}%", [query], limit))
            
            rows = cursor.fetchall()
            cursor.close()
            
            return [self._row_to_source(row) for row in rows]
        except Exception as e:
            print(f"Error searching sources: {e}")
            return []
    
    def get_source(self, source_id: str) -> Optional[Source]:
        """Get source by ID"""
        try:
            cursor = self.conn.cursor(cursor_factory=RealDictCursor)
            cursor.execute("SELECT * FROM research_sources WHERE id = %s", (source_id,))
            row = cursor.fetchone()
            cursor.close()
            
            return self._row_to_source(row) if row else None
        except Exception as e:
            print(f"Error getting source: {e}")
            return None
    
    @staticmethod
    def _row_to_source(row: Dict) -> Source:
        """Convert database row to Source object"""
        return Source(
            id=row['id'],
            url=row['url'],
            title=row['title'],
            authors=row['authors'] or [],
            published_date=row['published_date'],
            accessed_date=row['accessed_date'],
            content=row['content'],
            content_hash=row['content_hash'],
            domain=row['domain'],
            source_type=row['source_type'],
            credibility_score=row['credibility_score'],
            tags=row['tags'] or [],
            created_at=row['created_at']
        )

# ============================================================================
# MESH NETWORK INTEGRATION
# ============================================================================

class ResearchMesh:
    """P2P mesh for sharing research sources"""
    
    def __init__(self, redis_url: str):
        self.redis_client = redis.from_url(redis_url)
        self.node_id = str(uuid.uuid4())[:12]
    
    async def broadcast_source(self, source: Source):
        """Broadcast source to mesh peers"""
        key = f"research:source:{source.id}"
        self.redis_client.set(key, json.dumps(asdict(source), default=str), ex=86400)
        print(f"Broadcasted source {source.id} to mesh")
    
    async def search_mesh(self, query: str) -> List[Source]:
        """Search mesh peers for sources"""
        pattern = "research:source:*"
        keys = self.redis_client.keys(pattern)
        
        sources = []
        for key in keys:
            data = self.redis_client.get(key)
            if data:
                source_dict = json.loads(data)
                if query.lower() in source_dict.get('title', '').lower():
                    sources.append(Source(**source_dict))
        
        return sources

# ============================================================================
# FASTAPI SERVER
# ============================================================================

app = FastAPI(title="GEOLOGOS Research Engine")

# Global instances
scraper = None
db = None
mesh = None
citation_gen = CitationGenerator()

class URLRequest(BaseModel):
    url: str
    tags: list = []

class SearchRequest(BaseModel):
    query: str
    limit: int = 20

class CitationRequest(BaseModel):
    source_id: str
    format: str

@app.on_event("startup")
async def startup():
    global scraper, db, mesh
    scraper = WebScraper()
    await scraper.init_session()
    db = ResearchDatabase(DATABASE_URL)
    db.connect()
    mesh = ResearchMesh(REDIS_URL)

@app.on_event("shutdown")
async def shutdown():
    global scraper
    if scraper:
        await scraper.close_session()

@app.post("/api/v1/research/scrape")
async def scrape_url(request: URLRequest):
    """Scrape URL and add to research database"""
    if not scraper:
        raise HTTPException(status_code=500, detail="Scraper not initialized")
    
    scraped = await scraper.scrape_url(request.url)
    
    if "error" in scraped:
        raise HTTPException(status_code=400, detail=scraped["error"])
    
    # Create source
    content_hash = hashlib.sha256(scraped["content"].encode()).hexdigest()
    source = Source(
        id=str(uuid.uuid4()),
        url=scraped["url"],
        title=scraped["title"],
        authors=scraped["authors"],
        published_date=scraped["published_date"],
        accessed_date=datetime.utcnow().isoformat(),
        content=scraped["content"],
        content_hash=content_hash,
        domain=scraped["domain"],
        source_type="webpage",
        credibility_score=0.7,
        tags=request.tags,
        created_at=datetime.utcnow().isoformat()
    )
    
    # Add to database
    db.add_source(source)
    
    # Broadcast to mesh
    await mesh.broadcast_source(source)
    
    return {"id": source.id, "title": source.title, "domain": source.domain}

@app.get("/api/v1/research/search")
async def search_research(query: str, limit: int = 20, include_mesh: bool = True):
    """Search research sources"""
    # Search local database
    sources = db.search_sources(query, limit)
    
    # Search mesh if enabled
    mesh_sources = []
    if include_mesh:
        mesh_sources = await mesh.search_mesh(query)
    
    all_sources = sources + mesh_sources[:5]  # Include top 5 from mesh
    
    return {
        "query": query,
        "results": [asdict(s) for s in all_sources],
        "total": len(all_sources)
    }

@app.get("/api/v1/research/source/{source_id}")
async def get_source(source_id: str):
    """Get specific source"""
    source = db.get_source(source_id)
    if not source:
        raise HTTPException(status_code=404, detail="Source not found")
    
    return asdict(source)

@app.post("/api/v1/research/cite")
async def generate_citation(request: CitationRequest):
    """Generate citation for source"""
    source = db.get_source(request.source_id)
    if not source:
        raise HTTPException(status_code=404, detail="Source not found")
    
    format_enum = CitationFormat[request.format.upper()]
    citation = citation_gen.generate(source, format_enum)
    
    return asdict(citation)

@app.get("/health")
async def health():
    return {"status": "healthy", "service": "research-engine"}

# ============================================================================
# RUN
# ============================================================================

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8001)