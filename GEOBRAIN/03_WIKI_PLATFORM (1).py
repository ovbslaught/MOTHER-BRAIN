#!/usr/bin/env python3
"""
GEOLOGOS WIKI: Collaborative Knowledge Base - Production Ready
Git-backed, version control, multi-user editing, semantic linking
Deploy: python wiki.py
"""

import json
import os
import hashlib
from datetime import datetime
from typing import List, Dict, Optional, Tuple
from pathlib import Path
import subprocess
from dataclasses import dataclass, asdict

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import git
from git import Repo

# ============================================================================
# CONFIGURATION
# ============================================================================

WIKI_REPO_PATH = "./geologos-wiki"
WIKI_PAGES_DIR = "pages"
WIKI_ASSETS_DIR = "assets"

# ============================================================================
# DATA STRUCTURES
# ============================================================================

@dataclass
class WikiPage:
    """Wiki page representation"""
    title: str
    slug: str
    content: str
    author: str
    created_at: str
    updated_at: str
    tags: List[str]
    references: List[str]  # Links to other pages/sources
    version: int
    is_published: bool

@dataclass
class WikiDiff:
    """Page version diff"""
    version_a: int
    version_b: int
    changes: str
    author: str
    timestamp: str

# ============================================================================
# GIT BACKEND
# ============================================================================

class WikiGitBackend:
    """Git-based version control for wiki pages"""
    
    def __init__(self, repo_path: str = WIKI_REPO_PATH):
        self.repo_path = repo_path
        self.pages_dir = os.path.join(repo_path, WIKI_PAGES_DIR)
        self.assets_dir = os.path.join(repo_path, WIKI_ASSETS_DIR)
        
        # Initialize repo if not exists
        if not os.path.exists(repo_path):
            self._init_repo()
        
        self.repo = Repo(repo_path)
    
    def _init_repo(self):
        """Initialize git repository"""
        os.makedirs(self.pages_dir, exist_ok=True)
        os.makedirs(self.assets_dir, exist_ok=True)
        
        repo = Repo.init(self.repo_path)
        
        # Create initial commit
        Path(os.path.join(self.repo_path, ".gitignore")).write_text("*.pyc\n.DS_Store\n")
        repo.index.add([".gitignore"])
        repo.index.commit("Initial commit")
    
    def save_page(self, page: WikiPage, author: str, commit_message: str) -> str:
        """Save page to git"""
        page_file = os.path.join(self.pages_dir, f"{page.slug}.md")
        
        # Create frontmatter
        frontmatter = f"""---
title: {page.title}
slug: {page.slug}
author: {author}
created_at: {page.created_at}
updated_at: {page.updated_at}
tags: {json.dumps(page.tags)}
references: {json.dumps(page.references)}
published: {str(page.is_published).lower()}
---

{page.content}
"""
        
        # Write file
        Path(page_file).write_text(frontmatter)
        
        # Stage and commit
        self.repo.index.add([page_file])
        self.repo.index.commit(f"{commit_message} (by {author})")
        
        # Get commit hash
        return self.repo.head.commit.hexsha
    
    def get_page_history(self, slug: str) -> List[Dict]:
        """Get page commit history"""
        page_file = f"{WIKI_PAGES_DIR}/{slug}.md"
        
        history = []
        for commit in self.repo.iter_commits(paths=page_file):
            history.append({
                "sha": commit.hexsha[:7],
                "author": commit.author.name,
                "date": datetime.fromtimestamp(commit.committed_date).isoformat(),
                "message": commit.message.strip()
            })
        
        return history
    
    def get_page_version(self, slug: str, commit: str) -> Optional[str]:
        """Get specific version of page"""
        try:
            return self.repo.odb.stream(
                self.repo.commit(commit).tree[f"{WIKI_PAGES_DIR}/{slug}.md"].binsha
            ).read().decode()
        except:
            return None
    
    def get_diff(self, slug: str, commit_a: str, commit_b: str) -> str:
        """Get diff between two versions"""
        try:
            diffs = self.repo.commit(commit_a).diff(commit_b)
            diff_text = ""
            for diff_item in diffs:
                diff_text += str(diff_item)
            return diff_text
        except:
            return ""

# ============================================================================
# WIKI ENGINE
# ============================================================================

class WikiEngine:
    """Core wiki functionality"""
    
    def __init__(self):
        self.git = WikiGitBackend()
        self.pages: Dict[str, WikiPage] = {}
        self.index: Dict[str, List[str]] = {}  # slug -> referenced_slugs
        self._load_pages()
    
    def _load_pages(self):
        """Load all pages from git"""
        for page_file in Path(self.git.pages_dir).glob("*.md"):
            content = page_file.read_text()
            slug = page_file.stem
            
            # Parse frontmatter
            if content.startswith("---"):
                parts = content.split("---", 2)
                if len(parts) >= 3:
                    frontmatter = parts[1]
                    page_content = parts[2].strip()
                    
                    # Parse YAML-like frontmatter
                    meta = {}
                    for line in frontmatter.split("\n"):
                        if ":" in line:
                            key, value = line.split(":", 1)
                            meta[key.strip()] = value.strip()
                    
                    page = WikiPage(
                        title=meta.get("title", slug),
                        slug=slug,
                        content=page_content,
                        author=meta.get("author", "Unknown"),
                        created_at=meta.get("created_at", datetime.utcnow().isoformat()),
                        updated_at=meta.get("updated_at", datetime.utcnow().isoformat()),
                        tags=json.loads(meta.get("tags", "[]")),
                        references=json.loads(meta.get("references", "[]")),
                        version=len(self.git.get_page_history(slug)),
                        is_published=meta.get("published", "true").lower() == "true"
                    )
                    
                    self.pages[slug] = page
    
    def create_page(self, title: str, content: str, author: str, tags: List[str], 
                   references: List[str]) -> WikiPage:
        """Create new wiki page"""
        slug = title.lower().replace(" ", "-")
        
        if slug in self.pages:
            raise ValueError(f"Page '{slug}' already exists")
        
        page = WikiPage(
            title=title,
            slug=slug,
            content=content,
            author=author,
            created_at=datetime.utcnow().isoformat(),
            updated_at=datetime.utcnow().isoformat(),
            tags=tags,
            references=references,
            version=1,
            is_published=True
        )
        
        self.pages[slug] = page
        self.git.save_page(page, author, f"Create page: {title}")
        
        return page
    
    def edit_page(self, slug: str, content: str, author: str, summary: str) -> WikiPage:
        """Edit existing wiki page"""
        if slug not in self.pages:
            raise ValueError(f"Page '{slug}' not found")
        
        page = self.pages[slug]
        page.content = content
        page.author = author
        page.updated_at = datetime.utcnow().isoformat()
        page.version += 1
        
        self.git.save_page(page, author, f"Edit: {summary}")
        
        return page
    
    def get_page(self, slug: str) -> Optional[WikiPage]:
        """Get page by slug"""
        return self.pages.get(slug)
    
    def search_pages(self, query: str) -> List[WikiPage]:
        """Search pages by title, content, or tags"""
        results = []
        query_lower = query.lower()
        
        for page in self.pages.values():
            if (query_lower in page.title.lower() or
                query_lower in page.content.lower() or
                any(query_lower in tag.lower() for tag in page.tags)):
                results.append(page)
        
        return results
    
    def get_backlinks(self, slug: str) -> List[str]:
        """Get pages that link to this page"""
        backlinks = []
        for page in self.pages.values():
            if slug in page.references:
                backlinks.append(page.slug)
        return backlinks
    
    def get_related_pages(self, slug: str, limit: int = 5) -> List[WikiPage]:
        """Get related pages (share tags or links)"""
        if slug not in self.pages:
            return []
        
        page = self.pages[slug]
        related = []
        
        for other_slug, other_page in self.pages.items():
            if other_slug == slug:
                continue
            
            # Calculate similarity
            shared_tags = len(set(page.tags) & set(other_page.tags))
            in_references = slug in other_page.references
            referenced = other_slug in page.references
            
            if shared_tags > 0 or in_references or referenced:
                related.append((other_page, shared_tags + (5 if in_references else 0) + (5 if referenced else 0)))
        
        # Sort by relevance
        related.sort(key=lambda x: x[1], reverse=True)
        
        return [p[0] for p in related[:limit]]
    
    def get_all_tags(self) -> Dict[str, int]:
        """Get all tags with counts"""
        tags = {}
        for page in self.pages.values():
            for tag in page.tags:
                tags[tag] = tags.get(tag, 0) + 1
        return tags
    
    def export_to_json(self) -> str:
        """Export entire wiki as JSON"""
        data = {
            "pages": {slug: asdict(page) for slug, page in self.pages.items()},
            "tags": self.get_all_tags(),
            "exported_at": datetime.utcnow().isoformat()
        }
        return json.dumps(data, indent=2)
    
    def create_toc(self) -> str:
        """Generate table of contents"""
        toc = "# Wiki Table of Contents\n\n"
        
        # Group by tag
        tags = self.get_all_tags()
        for tag in sorted(tags.keys()):
            toc += f"## {tag}\n"
            for page in self.pages.values():
                if tag in page.tags:
                    toc += f"- [{page.title}]({page.slug})\n"
            toc += "\n"
        
        return toc

# ============================================================================
# FASTAPI ENDPOINTS
# ============================================================================

app = FastAPI(title="GEOLOGOS Wiki")
wiki = WikiEngine()

class PageRequest(BaseModel):
    title: str
    content: str
    author: str
    tags: List[str] = []
    references: List[str] = []

class EditRequest(BaseModel):
    content: str
    author: str
    summary: str

@app.post("/api/v1/wiki/pages")
async def create_page(request: PageRequest):
    """Create new wiki page"""
    try:
        page = wiki.create_page(
            request.title,
            request.content,
            request.author,
            request.tags,
            request.references
        )
        return asdict(page)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.get("/api/v1/wiki/pages/{slug}")
async def get_page(slug: str):
    """Get wiki page"""
    page = wiki.get_page(slug)
    if not page:
        raise HTTPException(status_code=404, detail="Page not found")
    return asdict(page)

@app.put("/api/v1/wiki/pages/{slug}")
async def edit_page(slug: str, request: EditRequest):
    """Edit wiki page"""
    try:
        page = wiki.edit_page(slug, request.content, request.author, request.summary)
        return asdict(page)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.get("/api/v1/wiki/search")
async def search(q: str):
    """Search wiki pages"""
    results = wiki.search_pages(q)
    return [asdict(p) for p in results]

@app.get("/api/v1/wiki/tags")
async def get_tags():
    """Get all tags"""
    return wiki.get_all_tags()

@app.get("/api/v1/wiki/pages/{slug}/backlinks")
async def get_backlinks(slug: str):
    """Get pages linking to this page"""
    return wiki.get_backlinks(slug)

@app.get("/api/v1/wiki/pages/{slug}/related")
async def get_related(slug: str):
    """Get related pages"""
    related = wiki.get_related_pages(slug)
    return [asdict(p) for p in related]

@app.get("/api/v1/wiki/toc")
async def get_toc():
    """Get table of contents"""
    return {"toc": wiki.create_toc()}

@app.get("/api/v1/wiki/export")
async def export_wiki():
    """Export entire wiki as JSON"""
    return json.loads(wiki.export_to_json())

@app.get("/health")
async def health():
    return {"status": "healthy", "service": "wiki"}

# ============================================================================
# RUN
# ============================================================================

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8002)