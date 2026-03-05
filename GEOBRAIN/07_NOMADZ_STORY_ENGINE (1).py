#!/usr/bin/env python3
"""
GEOLOGOS NOMADZ STORY ENGINE: Production Ready
Interactive narrative generation + branching storytelling + canon management
Deploy: python story_engine.py
"""

import json
import uuid
from datetime import datetime
from typing import List, Dict, Optional, Tuple, Set
from dataclasses import dataclass, asdict
from enum import Enum
from collections import defaultdict

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import networkx as nx

# ============================================================================
# DATA STRUCTURES
# ============================================================================

class StoryNodeType(Enum):
    """Types of story nodes"""
    SCENE = "scene"
    CHOICE = "choice"
    EVENT = "event"
    REVELATION = "revelation"
    CLIMAX = "climax"
    RESOLUTION = "resolution"

class CharacterRole(Enum):
    """Character roles in story"""
    PROTAGONIST = "protagonist"
    ANTAGONIST = "antagonist"
    ALLY = "ally"
    MENTOR = "mentor"
    COMPANION = "companion"
    OBSTACLE = "obstacle"

@dataclass
class StoryNode:
    """Single story beat/scene"""
    id: str
    type: StoryNodeType
    title: str
    description: str
    content: str
    characters: List[str]  # character IDs
    locations: List[str]  # location IDs
    choices: List[str]  # next node IDs
    parent_node: Optional[str]
    timeline_point: int  # chronological order
    consequences: List[str]  # what changes after this node
    citations: List[str]  # research sources
    created_at: str

@dataclass
class StoryBranch:
    """Story branch/path"""
    id: str
    title: str
    description: str
    starting_node: str
    nodes: List[str]
    characters_involved: List[str]
    themes: List[str]
    word_count: int
    created_at: str

@dataclass
class StoryTimeline:
    """Story timeline for continuity"""
    id: str
    story_id: str
    events: Dict[int, str]  # timestamp -> event description
    branches_diverged: Dict[str, int]  # branch_id -> timestamp
    critical_points: List[Tuple[int, str]]  # (timestamp, description)
    created_at: str

@dataclass
class NomadzStory:
    """Complete Nomadz universe story"""
    id: str
    title: str
    universe: str  # "nomadz", "cosmos", etc
    description: str
    branches: List[StoryBranch]
    timeline: StoryTimeline
    nodes: Dict[str, StoryNode]
    relationships: Dict[str, List[str]]  # character/location connections
    tags: List[str]
    research_sources: List[str]
    canonical: bool
    created_at: str

# ============================================================================
# STORY ENGINE
# ============================================================================

class StoryEngine:
    """Narrative generation and branching"""
    
    def __init__(self):
        self.stories: Dict[str, NomadzStory] = {}
        self.story_graphs: Dict[str, nx.DiGraph] = {}
        self.canonical_paths: Dict[str, Set[str]] = defaultdict(set)
    
    def create_story(self, title: str, universe: str, description: str,
                    tags: List[str]) -> NomadzStory:
        """Create new story in Nomadz universe"""
        
        story_id = str(uuid.uuid4())
        timeline = StoryTimeline(
            id=str(uuid.uuid4()),
            story_id=story_id,
            events={},
            branches_diverged={},
            critical_points=[],
            created_at=datetime.utcnow().isoformat()
        )
        
        story = NomadzStory(
            id=story_id,
            title=title,
            universe=universe,
            description=description,
            branches=[],
            timeline=timeline,
            nodes={},
            relationships={},
            tags=tags,
            research_sources=[],
            canonical=False,
            created_at=datetime.utcnow().isoformat()
        )
        
        self.stories[story_id] = story
        self.story_graphs[story_id] = nx.DiGraph()
        
        return story
    
    def add_scene(self, story_id: str, node_type: StoryNodeType,
                 title: str, description: str, content: str,
                 characters: List[str], locations: List[str],
                 timeline_point: int = 0) -> StoryNode:
        """Add scene/node to story"""
        
        if story_id not in self.stories:
            raise ValueError(f"Story {story_id} not found")
        
        story = self.stories[story_id]
        node_id = str(uuid.uuid4())
        
        node = StoryNode(
            id=node_id,
            type=node_type,
            title=title,
            description=description,
            content=content,
            characters=characters,
            locations=locations,
            choices=[],
            parent_node=None,
            timeline_point=timeline_point,
            consequences=[],
            citations=[],
            created_at=datetime.utcnow().isoformat()
        )
        
        story.nodes[node_id] = node
        self.story_graphs[story_id].add_node(node_id, node=node)
        
        return node
    
    def add_choice(self, story_id: str, from_node_id: str, to_node_id: str,
                  choice_text: str, consequence: Optional[str] = None) -> bool:
        """Add choice/branching path"""
        
        if story_id not in self.stories:
            return False
        
        story = self.stories[story_id]
        
        if from_node_id not in story.nodes or to_node_id not in story.nodes:
            return False
        
        # Add edge to graph
        self.story_graphs[story_id].add_edge(from_node_id, to_node_id, 
                                            label=choice_text)
        
        # Update nodes
        story.nodes[from_node_id].choices.append(to_node_id)
        
        if consequence:
            story.nodes[to_node_id].consequences.append(consequence)
        
        return True
    
    def create_branch(self, story_id: str, title: str, 
                     starting_node_id: str,
                     description: str) -> StoryBranch:
        """Create story branch/path"""
        
        if story_id not in self.stories:
            raise ValueError(f"Story {story_id} not found")
        
        branch_id = str(uuid.uuid4())
        
        branch = StoryBranch(
            id=branch_id,
            title=title,
            description=description,
            starting_node=starting_node_id,
            nodes=[starting_node_id],
            characters_involved=[],
            themes=[],
            word_count=0,
            created_at=datetime.utcnow().isoformat()
        )
        
        self.stories[story_id].branches.append(branch)
        return branch
    
    def get_story_path(self, story_id: str, start_node_id: str,
                      end_node_id: Optional[str] = None) -> List[str]:
        """Get path through story graph"""
        
        if story_id not in self.story_graphs:
            return []
        
        try:
            if end_node_id:
                path = nx.shortest_path(
                    self.story_graphs[story_id],
                    start_node_id,
                    end_node_id
                )
            else:
                # Get all reachable nodes
                path = list(nx.descendants(
                    self.story_graphs[story_id],
                    start_node_id
                ))
            return path
        except nx.NetworkXNoPath:
            return []
    
    def check_continuity(self, story_id: str) -> List[str]:
        """Check story for continuity errors"""
        
        if story_id not in self.stories:
            return []
        
        story = self.stories[story_id]
        issues = []
        
        # Check for orphaned nodes
        all_nodes = set(story.nodes.keys())
        reachable = set()
        
        for node_id in story.nodes:
            reachable.update(nx.descendants(
                self.story_graphs[story_id],
                node_id
            ))
        
        orphaned = all_nodes - reachable
        if orphaned:
            issues.append(f"Orphaned nodes: {orphaned}")
        
        # Check for circular paths (loops)
        try:
            cycles = list(nx.simple_cycles(self.story_graphs[story_id]))
            if cycles:
                issues.append(f"Circular paths detected: {cycles}")
        except:
            pass
        
        # Check character consistency
        for node in story.nodes.values():
            for char in node.characters:
                if char not in story.relationships:
                    issues.append(f"Unknown character {char} in node {node.id}")
        
        return issues
    
    def mark_canonical(self, story_id: str, branch_id: str):
        """Mark branch as canonical"""
        
        if story_id not in self.stories:
            return
        
        story = self.stories[story_id]
        story.canonical = True
        
        for branch in story.branches:
            if branch.id == branch_id:
                self.canonical_paths[story_id].update(branch.nodes)
    
    def generate_summary(self, story_id: str) -> str:
        """Generate story summary"""
        
        if story_id not in self.stories:
            return ""
        
        story = self.stories[story_id]
        summary = f"# {story.title}\n\n"
        summary += f"Universe: {story.universe}\n"
        summary += f"Description: {story.description}\n\n"
        
        summary += f"## Key Characters\n"
        for char in list(story.relationships.keys())[:5]:
            summary += f"- {char}\n"
        
        summary += f"\n## Story Branches\n"
        for branch in story.branches:
            summary += f"- {branch.title}: {len(branch.nodes)} scenes\n"
        
        summary += f"\n## Themes\n"
        for tag in story.tags:
            summary += f"- {tag}\n"
        
        return summary

# ============================================================================
# FASTAPI SERVER
# ============================================================================

app = FastAPI(title="GEOLOGOS Nomadz Story Engine")
engine = None

class StoryRequest(BaseModel):
    title: str
    universe: str
    description: str
    tags: List[str] = []

class SceneRequest(BaseModel):
    node_type: str
    title: str
    description: str
    content: str
    characters: List[str] = []
    locations: List[str] = []
    timeline_point: int = 0

class ChoiceRequest(BaseModel):
    from_node_id: str
    to_node_id: str
    choice_text: str
    consequence: Optional[str] = None

class BranchRequest(BaseModel):
    title: str
    starting_node_id: str
    description: str

@app.on_event("startup")
async def startup():
    global engine
    engine = StoryEngine()

@app.post("/api/v1/stories")
async def create_story(request: StoryRequest):
    """Create new story"""
    story = engine.create_story(
        request.title,
        request.universe,
        request.description,
        request.tags
    )
    return asdict(story)

@app.post("/api/v1/stories/{story_id}/scenes")
async def add_scene(story_id: str, request: SceneRequest):
    """Add scene to story"""
    try:
        node_type = StoryNodeType[request.node_type.upper()]
        node = engine.add_scene(
            story_id,
            node_type,
            request.title,
            request.description,
            request.content,
            request.characters,
            request.locations,
            request.timeline_point
        )
        return asdict(node)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.post("/api/v1/stories/{story_id}/choices")
async def add_choice(story_id: str, request: ChoiceRequest):
    """Add choice/branching"""
    success = engine.add_choice(
        story_id,
        request.from_node_id,
        request.to_node_id,
        request.choice_text,
        request.consequence
    )
    
    if not success:
        raise HTTPException(status_code=400, detail="Failed to add choice")
    
    return {"status": "choice added"}

@app.post("/api/v1/stories/{story_id}/branches")
async def create_branch(story_id: str, request: BranchRequest):
    """Create story branch"""
    try:
        branch = engine.create_branch(
            story_id,
            request.title,
            request.starting_node_id,
            request.description
        )
        return asdict(branch)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.get("/api/v1/stories/{story_id}")
async def get_story(story_id: str):
    """Get story"""
    if story_id not in engine.stories:
        raise HTTPException(status_code=404, detail="Story not found")
    return asdict(engine.stories[story_id])

@app.get("/api/v1/stories/{story_id}/check")
async def check_continuity(story_id: str):
    """Check story continuity"""
    issues = engine.check_continuity(story_id)
    return {"issues": issues, "valid": len(issues) == 0}

@app.get("/api/v1/stories/{story_id}/summary")
async def get_summary(story_id: str):
    """Get story summary"""
    summary = engine.generate_summary(story_id)
    return {"summary": summary}

@app.post("/api/v1/stories/{story_id}/canonical/{branch_id}")
async def mark_canonical(story_id: str, branch_id: str):
    """Mark branch as canonical"""
    engine.mark_canonical(story_id, branch_id)
    return {"status": "marked canonical"}

@app.get("/health")
async def health():
    return {"status": "healthy", "service": "story-engine"}

# ============================================================================
# RUN
# ============================================================================

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8006)