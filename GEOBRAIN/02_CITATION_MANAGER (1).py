#!/usr/bin/env python3
"""
GEOLOGOS CITATION MANAGER: Production Ready
Multi-format citations, bibliography management, automatic generation
"""

from enum import Enum
from dataclasses import dataclass
from typing import List, Dict, Optional
from datetime import datetime
import json

class CitationStyle(Enum):
    """Supported citation styles"""
    APA_7 = "apa7"
    MLA_9 = "mla9"
    CHICAGO_17 = "chicago17"
    HARVARD = "harvard"
    IEEE = "ieee"
    BIBTEX = "bibtex"
    CSL_JSON = "csl-json"

@dataclass
class BibliographyEntry:
    """Single bibliography entry"""
    id: str
    authors: List[str]
    title: str
    publication_year: int
    url: Optional[str] = None
    doi: Optional[str] = None
    isbn: Optional[str] = None
    publisher: Optional[str] = None
    journal: Optional[str] = None
    volume: Optional[str] = None
    issue: Optional[str] = None
    pages: Optional[str] = None
    accessed_date: Optional[str] = None
    source_type: str = "webpage"  # book, journal, website, etc

class CitationManager:
    """Manage citations across multiple formats"""
    
    def __init__(self):
        self.entries: Dict[str, BibliographyEntry] = {}
        self.citations_cache = {}
    
    def add_entry(self, entry: BibliographyEntry) -> str:
        """Add bibliography entry"""
        self.entries[entry.id] = entry
        return entry.id
    
    def generate_citation(self, entry_id: str, style: CitationStyle) -> str:
        """Generate citation in specified style"""
        if entry_id not in self.entries:
            raise ValueError(f"Entry {entry_id} not found")
        
        entry = self.entries[entry_id]
        
        if style == CitationStyle.APA_7:
            return self._format_apa7(entry)
        elif style == CitationStyle.MLA_9:
            return self._format_mla9(entry)
        elif style == CitationStyle.CHICAGO_17:
            return self._format_chicago17(entry)
        elif style == CitationStyle.HARVARD:
            return self._format_harvard(entry)
        elif style == CitationStyle.IEEE:
            return self._format_ieee(entry)
        elif style == CitationStyle.BIBTEX:
            return self._format_bibtex(entry)
        elif style == CitationStyle.CSL_JSON:
            return self._format_csl_json(entry)
        
        return self._format_apa7(entry)
    
    def generate_in_text_citation(self, entry_id: str) -> str:
        """Generate in-text citation (parenthetical)"""
        if entry_id not in self.entries:
            raise ValueError(f"Entry {entry_id} not found")
        
        entry = self.entries[entry_id]
        first_author = entry.authors[0] if entry.authors else "Unknown"
        
        # Extract last name
        last_name = first_author.split()[-1] if " " in first_author else first_author
        
        return f"({last_name}, {entry.publication_year})"
    
    def generate_bibliography(self, entry_ids: List[str], style: CitationStyle) -> str:
        """Generate formatted bibliography"""
        citations = []
        for entry_id in entry_ids:
            citations.append(self.generate_citation(entry_id, style))
        
        # Sort alphabetically by first author last name
        citations.sort()
        
        return "\n".join(citations)
    
    # ========================================================================
    # FORMAT IMPLEMENTATIONS
    # ========================================================================
    
    def _format_apa7(self, entry: BibliographyEntry) -> str:
        """APA 7th Edition format"""
        authors = self._format_authors_apa(entry.authors)
        
        if entry.source_type == "journal":
            return (f"{authors} ({entry.publication_year}). {entry.title}. "
                   f"{entry.journal}, {entry.volume}({entry.issue}), {entry.pages}. "
                   f"https://doi.org/{entry.doi}" if entry.doi else entry.url or "")
        elif entry.source_type == "book":
            return (f"{authors} ({entry.publication_year}). {entry.title}. "
                   f"{entry.publisher}.")
        else:  # website
            return (f"{authors} ({entry.publication_year}). {entry.title}. "
                   f"Retrieved {entry.accessed_date} from {entry.url}")
    
    def _format_mla9(self, entry: BibliographyEntry) -> str:
        """MLA 9th Edition format"""
        authors = self._format_authors_mla(entry.authors)
        
        if entry.source_type == "journal":
            return (f"{authors}. \"{entry.title}.\" {entry.journal}, "
                   f"vol. {entry.volume}, no. {entry.issue}, {entry.publication_year}, "
                   f"pp. {entry.pages}.")
        elif entry.source_type == "book":
            return f"{authors}. {entry.title}. {entry.publisher}, {entry.publication_year}."
        else:  # website
            return (f"{authors}. \"{entry.title}.\" Accessed {entry.accessed_date}, "
                   f"{entry.url}")
    
    def _format_chicago17(self, entry: BibliographyEntry) -> str:
        """Chicago Manual of Style 17th Edition (Notes and Bibliography)"""
        authors = self._format_authors_chicago(entry.authors)
        
        if entry.source_type == "journal":
            return (f"{authors}. \"{entry.title}.\" {entry.journal} {entry.volume}, "
                   f"no. {entry.issue} ({entry.publication_year}): {entry.pages}.")
        elif entry.source_type == "book":
            return f"{authors}. {entry.title}. {entry.publisher}, {entry.publication_year}."
        else:  # website
            return (f"{authors}. \"{entry.title}.\" Accessed {entry.accessed_date}. "
                   f"{entry.url}")
    
    def _format_harvard(self, entry: BibliographyEntry) -> str:
        """Harvard referencing style"""
        authors = self._format_authors_harvard(entry.authors)
        
        if entry.source_type == "journal":
            return (f"{authors} {entry.publication_year}, '{entry.title}', "
                   f"{entry.journal}, {entry.volume}({entry.issue}), pp.{entry.pages}.")
        elif entry.source_type == "book":
            return f"{authors} {entry.publication_year}, {entry.title}. {entry.publisher}."
        else:  # website
            return (f"{authors} {entry.publication_year}, {entry.title}. "
                   f"Available at: {entry.url} (Accessed: {entry.accessed_date}).")
    
    def _format_ieee(self, entry: BibliographyEntry) -> str:
        """IEEE citation style"""
        authors_short = ", ".join([a.split()[-1] for a in entry.authors[:3]])
        
        if entry.source_type == "journal":
            return (f"[1] {authors_short}, \"{entry.title},\" {entry.journal}, "
                   f"vol. {entry.volume}, no. {entry.issue}, pp. {entry.pages}, "
                   f"{entry.publication_year}.")
        else:  # website
            return (f"[1] {authors_short}, \"{entry.title},\" {entry.publication_year}. "
                   f"[Online]. Available: {entry.url}. [Accessed: {entry.accessed_date}].")
    
    def _format_bibtex(self, entry: BibliographyEntry) -> str:
        """BibTeX format"""
        citekey = f"{entry.authors[0].split()[-1].lower()}{entry.publication_year}"
        authors = " and ".join(entry.authors)
        
        if entry.source_type == "journal":
            return (f"@article{{{citekey},\n"
                   f"  author = {{{authors}}},\n"
                   f"  title = {{{entry.title}}},\n"
                   f"  journal = {{{entry.journal}}},\n"
                   f"  volume = {{{entry.volume}}},\n"
                   f"  number = {{{entry.issue}}},\n"
                   f"  pages = {{{entry.pages}}},\n"
                   f"  year = {{{entry.publication_year}}},\n"
                   f"  doi = {{{entry.doi}}}\n"
                   f"}}")
        elif entry.source_type == "book":
            return (f"@book{{{citekey},\n"
                   f"  author = {{{authors}}},\n"
                   f"  title = {{{entry.title}}},\n"
                   f"  publisher = {{{entry.publisher}}},\n"
                   f"  year = {{{entry.publication_year}}},\n"
                   f"  isbn = {{{entry.isbn}}}\n"
                   f"}}")
        else:
            return (f"@misc{{{citekey},\n"
                   f"  author = {{{authors}}},\n"
                   f"  title = {{{entry.title}}},\n"
                   f"  url = {{{entry.url}}},\n"
                   f"  year = {{{entry.publication_year}}},\n"
                   f"  note = {{Accessed: {entry.accessed_date}}}\n"
                   f"}}")
    
    def _format_csl_json(self, entry: BibliographyEntry) -> str:
        """CSL JSON format (for Zotero, Mendeley, etc)"""
        csl_type = {
            "book": "book",
            "journal": "article-journal",
            "website": "webpage"
        }.get(entry.source_type, "webpage")
        
        csl_entry = {
            "id": entry.id,
            "type": csl_type,
            "author": [{"family": a.split()[-1], "given": " ".join(a.split()[:-1])} 
                      for a in entry.authors],
            "title": entry.title,
            "issued": {"date-parts": [[entry.publication_year]]},
        }
        
        if entry.url:
            csl_entry["URL"] = entry.url
        if entry.doi:
            csl_entry["DOI"] = entry.doi
        if entry.journal:
            csl_entry["container-title"] = entry.journal
        if entry.volume:
            csl_entry["volume"] = entry.volume
        if entry.issue:
            csl_entry["issue"] = entry.issue
        if entry.pages:
            csl_entry["page"] = entry.pages
        
        return json.dumps(csl_entry, indent=2)
    
    # ========================================================================
    # HELPER METHODS
    # ========================================================================
    
    def _format_authors_apa(self, authors: List[str]) -> str:
        """Format authors for APA style"""
        if not authors:
            return "Unknown"
        if len(authors) == 1:
            return authors[0]
        if len(authors) == 2:
            return f"{authors[0]} & {authors[1]}"
        return f"{authors[0]} et al."
    
    def _format_authors_mla(self, authors: List[str]) -> str:
        """Format authors for MLA style"""
        if not authors:
            return "Unknown"
        if len(authors) == 1:
            return authors[0]
        return f"{authors[0]}, et al."
    
    def _format_authors_chicago(self, authors: List[str]) -> str:
        """Format authors for Chicago style"""
        if not authors:
            return "Unknown"
        if len(authors) == 1:
            return authors[0]
        return f"{authors[0]}, and {', and '.join(authors[1:])}"
    
    def _format_authors_harvard(self, authors: List[str]) -> str:
        """Format authors for Harvard style"""
        if not authors:
            return "Unknown"
        if len(authors) == 1:
            return authors[0]
        if len(authors) == 2:
            return f"{authors[0]} and {authors[1]}"
        return f"{authors[0]}, et al."

# ============================================================================
# EXAMPLE USAGE
# ============================================================================

if __name__ == "__main__":
    manager = CitationManager()
    
    # Add sample entries
    entry1 = BibliographyEntry(
        id="smith2023",
        authors=["John Smith", "Jane Doe"],
        title="Understanding Machine Learning",
        publication_year=2023,
        journal="AI Research",
        volume="12",
        issue="3",
        pages="45-67",
        source_type="journal",
        doi="10.1234/ai.2023.12345"
    )
    
    entry2 = BibliographyEntry(
        id="jones2022",
        authors=["Bob Jones"],
        title="Web Development Best Practices",
        publication_year=2022,
        publisher="Tech Press",
        source_type="book",
        isbn="978-1234567890"
    )
    
    manager.add_entry(entry1)
    manager.add_entry(entry2)
    
    # Generate citations in different formats
    print("=== APA Format ===")
    print(manager.generate_citation("smith2023", CitationStyle.APA_7))
    print()
    
    print("=== MLA Format ===")
    print(manager.generate_citation("smith2023", CitationStyle.MLA_9))
    print()
    
    print("=== Chicago Format ===")
    print(manager.generate_citation("smith2023", CitationStyle.CHICAGO_17))
    print()
    
    print("=== Harvard Format ===")
    print(manager.generate_citation("smith2023", CitationStyle.HARVARD))
    print()
    
    print("=== IEEE Format ===")
    print(manager.generate_citation("smith2023", CitationStyle.IEEE))
    print()
    
    print("=== BibTeX Format ===")
    print(manager.generate_citation("smith2023", CitationStyle.BIBTEX))
    print()
    
    print("=== In-Text Citation ===")
    print(manager.generate_in_text_citation("smith2023"))
    print()
    
    print("=== Full Bibliography ===")
    print(manager.generate_bibliography(["smith2023", "jones2022"], CitationStyle.APA_7))