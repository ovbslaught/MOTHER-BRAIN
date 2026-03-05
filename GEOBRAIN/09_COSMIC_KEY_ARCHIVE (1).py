#!/usr/bin/env python3
"""
GEOLOGOS COSMIC KEY ARCHIVE + CHARACTER GENERATOR
Sacred knowledge repository + AI character generation
Deploy: python cosmic_archive.py
"""

import json
import uuid
from datetime import datetime
from typing import List, Dict, Optional
from dataclasses import dataclass, asdict
from enum import Enum
import hashlib

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

# ============================================================================
# COSMIC KEY ARCHIVE
# ============================================================================

class KnowledgeCategory(Enum):
    """Categories of cosmic knowledge"""
    CREATION = "creation"
    DIMENSIONS = "dimensions"
    CONSCIOUSNESS = "consciousness"
    ETHICS = "ethics"
    TECHNOLOGY = "technology"
    SPIRITUALITY = "spirituality"
    HISTORY = "history"
    PROPHECY = "prophecy"

class AccessLevel(Enum):
    """Access levels for knowledge"""
    PUBLIC = "public"
    RESTRICTED = "restricted"
    SACRED = "sacred"
    CLASSIFIED = "classified"

@dataclass
class CosmicTruth:
    """Single cosmic truth/knowledge"""
    id: str
    title: str
    category: KnowledgeCategory
    content: str
    author: str  # Who documented it
    access_level: AccessLevel
    related_truths: List[str]  # IDs of related truths
    verified: bool
    verification_sources: List[str]
    implications: List[str]  # Story implications
    created_at: str
    updated_at: str

@dataclass
class CosmicArchive:
    """Cosmic Key knowledge archive"""
    id: str
    name: str
    truths: Dict[str, CosmicTruth]
    categories: Dict[str, List[str]]  # category -> truth IDs
    access_log: List[Dict]  # Who accessed what when
    encrypted: bool
    encryption_key: Optional[str]
    mesh_peers: List[str]  # P2P mesh nodes with copies
    last_sync: str
    created_at: str

class CosmicKeyArchive:
    """Manage Cosmic Key knowledge"""
    
    def __init__(self):
        self.archives: Dict[str, CosmicArchive] = {}
        self.truth_index: Dict[str, str] = {}  # truth_id -> archive_id
    
    def create_archive(self, name: str, encrypted: bool = True) -> CosmicArchive:
        """Create new Cosmic Key archive"""
        
        archive = CosmicArchive(
            id=str(uuid.uuid4()),
            name=name,
            truths={},
            categories={k.value: [] for k in KnowledgeCategory},
            access_log=[],
            encrypted=encrypted,
            encryption_key=self._generate_encryption_key() if encrypted else None,
            mesh_peers=[],
            last_sync=datetime.utcnow().isoformat(),
            created_at=datetime.utcnow().isoformat()
        )
        
        self.archives[archive.id] = archive
        return archive
    
    def add_truth(self, archive_id: str, title: str, category: KnowledgeCategory,
                 content: str, author: str,
                 access_level: AccessLevel = AccessLevel.SACRED) -> CosmicTruth:
        """Add cosmic truth to archive"""
        
        if archive_id not in self.archives:
            raise ValueError(f"Archive {archive_id} not found")
        
        archive = self.archives[archive_id]
        truth_id = str(uuid.uuid4())
        
        truth = CosmicTruth(
            id=truth_id,
            title=title,
            category=category,
            content=content,
            author=author,
            access_level=access_level,
            related_truths=[],
            verified=False,
            verification_sources=[],
            implications=[],
            created_at=datetime.utcnow().isoformat(),
            updated_at=datetime.utcnow().isoformat()
        )
        
        archive.truths[truth_id] = truth
        archive.categories[category.value].append(truth_id)
        self.truth_index[truth_id] = archive_id
        
        return truth
    
    def link_truths(self, archive_id: str, truth_a_id: str, truth_b_id: str):
        """Link two truths together"""
        
        if archive_id not in self.archives:
            return
        
        archive = self.archives[archive_id]
        
        if truth_a_id in archive.truths and truth_b_id in archive.truths:
            archive.truths[truth_a_id].related_truths.append(truth_b_id)
            archive.truths[truth_b_id].related_truths.append(truth_a_id)
    
    def verify_truth(self, archive_id: str, truth_id: str,
                    verification_sources: List[str]):
        """Mark truth as verified"""
        
        if archive_id not in self.archives:
            return
        
        archive = self.archives[archive_id]
        
        if truth_id in archive.truths:
            archive.truths[truth_id].verified = True
            archive.truths[truth_id].verification_sources = verification_sources
    
    def add_implication(self, archive_id: str, truth_id: str, implication: str):
        """Add story implication of truth"""
        
        if archive_id not in self.archives:
            return
        
        archive = self.archives[archive_id]
        
        if truth_id in archive.truths:
            archive.truths[truth_id].implications.append(implication)
    
    def get_truths_by_category(self, archive_id: str,
                              category: KnowledgeCategory) -> List[CosmicTruth]:
        """Get all truths in category"""
        
        if archive_id not in self.archives:
            return []
        
        archive = self.archives[archive_id]
        truth_ids = archive.categories.get(category.value, [])
        
        return [archive.truths[tid] for tid in truth_ids if tid in archive.truths]
    
    def export_archive(self, archive_id: str) -> str:
        """Export archive as JSON"""
        
        if archive_id not in self.archives:
            return ""
        
        archive = self.archives[archive_id]
        export_data = {
            "name": archive.name,
            "truths": {k: asdict(v) for k, v in archive.truths.items()},
            "categories": archive.categories,
            "exported_at": datetime.utcnow().isoformat()
        }
        
        return json.dumps(export_data, indent=2, default=str)
    
    def _generate_encryption_key(self) -> str:
        """Generate encryption key"""
        return hashlib.sha256(str(uuid.uuid4()).encode()).hexdigest()

# ============================================================================
# CHARACTER GENERATOR
# ============================================================================

class CharacterArchetype(Enum):
    """Character archetypes"""
    HERO = "hero"
    MENTOR = "mentor"
    TRICKSTER = "trickster"
    SHADOW = "shadow"
    INNOCENT = "innocent"
    LOVER = "lover"
    SAGE = "sage"
    MAGICIAN = "magician"
    EVERYMAN = "everyman"
    EXPLORER = "explorer"

@dataclass
class CharacterTraits:
    """Character personality traits"""
    strengths: List[str]
    weaknesses: List[str]
    motivations: List[str]
    fears: List[str]
    values: List[str]

@dataclass
class Character:
    """AI-generated character"""
    id: str
    name: str
    archetype: CharacterArchetype
    age: int
    origin: str  # Where they're from
    traits: CharacterTraits
    background: str
    appearance: str
    abilities: List[str]
    relationships: Dict[str, str]  # character_id -> relationship type
    story_role: str  # What role in story
    created_at: str

class CharacterGenerator:
    """Generate characters for stories"""
    
    def __init__(self):
        self.characters: Dict[str, Character] = {}
        self.relationships: Dict[str, List[str]] = {}  # char_id -> related_char_ids
    
    def generate_character(self, name: str, archetype: CharacterArchetype,
                          age: int, origin: str, background: str,
                          appearance: str) -> Character:
        """Generate character"""
        
        char_id = str(uuid.uuid4())
        
        # Generate traits based on archetype
        traits = self._generate_traits_for_archetype(archetype)
        
        # Generate abilities
        abilities = self._generate_abilities(archetype, origin)
        
        character = Character(
            id=char_id,
            name=name,
            archetype=archetype,
            age=age,
            origin=origin,
            traits=traits,
            background=background,
            appearance=appearance,
            abilities=abilities,
            relationships={},
            story_role="",
            created_at=datetime.utcnow().isoformat()
        )
        
        self.characters[char_id] = character
        self.relationships[char_id] = []
        
        return character
    
    def _generate_traits_for_archetype(self, archetype: CharacterArchetype) -> CharacterTraits:
        """Generate traits for archetype"""
        
        archetypes = {
            CharacterArchetype.HERO: CharacterTraits(
                strengths=["courage", "determination", "leadership"],
                weaknesses=["overconfidence", "recklessness"],
                motivations=["justice", "protecting others", "growth"],
                fears=["failure", "losing loved ones"],
                values=["honor", "integrity", "courage"]
            ),
            CharacterArchetype.MENTOR: CharacterTraits(
                strengths=["wisdom", "patience", "guidance"],
                weaknesses=["detachment", "reluctance to act"],
                motivations=["teaching", "legacy"],
                fears=["irrelevance", "failure of students"],
                values=["knowledge", "growth", "tradition"]
            ),
            CharacterArchetype.TRICKSTER: CharacterTraits(
                strengths=["cleverness", "adaptability", "creativity"],
                weaknesses=["unreliability", "self-interest"],
                motivations=["freedom", "chaos", "profit"],
                fears=["being controlled", "predictability"],
                values=["cleverness", "freedom", "fun"]
            ),
            # Add more archetypes as needed
        }
        
        return archetypes.get(archetype, CharacterTraits(
            strengths=["adaptable"],
            weaknesses=["inexperienced"],
            motivations=["survival"],
            fears=["death"],
            values=["life"]
        ))
    
    def _generate_abilities(self, archetype: CharacterArchetype, origin: str) -> List[str]:
        """Generate abilities based on archetype and origin"""
        
        base_abilities = {
            CharacterArchetype.HERO: ["combat", "leadership", "problem-solving"],
            CharacterArchetype.MENTOR: ["teaching", "magical knowledge", "foresight"],
            CharacterArchetype.SAGE: ["research", "analysis", "wisdom"],
            CharacterArchetype.MAGICIAN: ["magic", "illusion", "manipulation"],
        }
        
        abilities = base_abilities.get(archetype, ["adaptability"])
        
        # Add origin-based abilities
        if origin == "nomadz":
            abilities.extend(["dimensional travel", "cosmic awareness"])
        elif origin == "earth":
            abilities.extend(["technology", "science"])
        
        return abilities
    
    def add_relationship(self, char_a_id: str, char_b_id: str,
                        relationship_type: str):
        """Add relationship between characters"""
        
        if char_a_id in self.characters and char_b_id in self.characters:
            self.characters[char_a_id].relationships[char_b_id] = relationship_type
            self.relationships[char_a_id].append(char_b_id)
    
    def generate_character_sheet(self, char_id: str) -> str:
        """Generate character sheet as text"""
        
        if char_id not in self.characters:
            return ""
        
        char = self.characters[char_id]
        sheet = f"# {char.name}\n\n"
        sheet += f"**Archetype:** {char.archetype.value}\n"
        sheet += f"**Age:** {char.age}\n"
        sheet += f"**Origin:** {char.origin}\n\n"
        
        sheet += "## Appearance\n"
        sheet += f"{char.appearance}\n\n"
        
        sheet += "## Background\n"
        sheet += f"{char.background}\n\n"
        
        sheet += "## Traits\n"
        sheet += f"**Strengths:** {', '.join(char.traits.strengths)}\n"
        sheet += f"**Weaknesses:** {', '.join(char.traits.weaknesses)}\n"
        sheet += f"**Motivations:** {', '.join(char.traits.motivations)}\n"
        sheet += f"**Fears:** {', '.join(char.traits.fears)}\n"
        sheet += f"**Values:** {', '.join(char.traits.values)}\n\n"
        
        sheet += "## Abilities\n"
        for ability in char.abilities:
            sheet += f"- {ability}\n"
        
        return sheet

# ============================================================================
# FASTAPI SERVER
# ============================================================================

app = FastAPI(title="GEOLOGOS Cosmic Archive + Character Generator")
cosmic_archive = None
character_gen = None

class ArchiveRequest(BaseModel):
    name: str
    encrypted: bool = True

class TruthRequest(BaseModel):
    title: str
    category: str
    content: str
    author: str
    access_level: str = "sacred"

class CharacterRequest(BaseModel):
    name: str
    archetype: str
    age: int
    origin: str
    background: str
    appearance: str

@app.on_event("startup")
async def startup():
    global cosmic_archive, character_gen
    cosmic_archive = CosmicKeyArchive()
    character_gen = CharacterGenerator()

@app.post("/api/v1/cosmic/archives")
async def create_archive(request: ArchiveRequest):
    """Create cosmic archive"""
    archive = cosmic_archive.create_archive(request.name, request.encrypted)
    return asdict(archive)

@app.post("/api/v1/cosmic/archives/{archive_id}/truths")
async def add_truth(archive_id: str, request: TruthRequest):
    """Add cosmic truth"""
    try:
        category = KnowledgeCategory[request.category.upper()]
        access_level = AccessLevel[request.access_level.upper()]
        
        truth = cosmic_archive.add_truth(
            archive_id,
            request.title,
            category,
            request.content,
            request.author,
            access_level
        )
        return asdict(truth)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.post("/api/v1/characters")
async def generate_character(request: CharacterRequest):
    """Generate character"""
    try:
        archetype = CharacterArchetype[request.archetype.upper()]
        
        character = character_gen.generate_character(
            request.name,
            archetype,
            request.age,
            request.origin,
            request.background,
            request.appearance
        )
        return asdict(character)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.get("/api/v1/characters/{char_id}/sheet")
async def get_character_sheet(char_id: str):
    """Get character sheet"""
    sheet = character_gen.generate_character_sheet(char_id)
    if not sheet:
        raise HTTPException(status_code=404, detail="Character not found")
    return {"sheet": sheet}

@app.get("/api/v1/cosmic/archives/{archive_id}/export")
async def export_archive(archive_id: str):
    """Export archive"""
    data = cosmic_archive.export_archive(archive_id)
    if not data:
        raise HTTPException(status_code=404, detail="Archive not found")
    return json.loads(data)

@app.get("/health")
async def health():
    return {"status": "healthy", "service": "cosmic-archive"}

# ============================================================================
# RUN
# ============================================================================

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8008)