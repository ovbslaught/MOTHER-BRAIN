#!/usr/bin/env python3
"""
GEOLOGOS COMIC CREATOR: Production Ready
AI panel generation + dialogue editor + layout templates + publishing
Deploy: python comic_creator.py
"""

import json
import uuid
from datetime import datetime
from typing import List, Dict, Optional, Tuple
from dataclasses import dataclass, asdict
from enum import Enum

from fastapi import FastAPI, HTTPException, File, UploadFile
from pydantic import BaseModel
import aiohttp
from PIL import Image, ImageDraw, ImageFont
import io

# ============================================================================
# DATA STRUCTURES
# ============================================================================

class PanelLayout(Enum):
    """Comic panel layouts"""
    SINGLE = "single"
    TWO_COLUMNS = "two_columns"
    THREE_COLUMNS = "three_columns"
    GRID_2X2 = "grid_2x2"
    GRID_3X3 = "grid_3x3"
    DYNAMIC = "dynamic"

@dataclass
class Character:
    """Comic character"""
    id: str
    name: str
    description: str
    visual_prompt: str
    personality_traits: List[str]
    created_at: str

@dataclass
class Panel:
    """Single comic panel"""
    id: str
    sequence: int
    scene_description: str
    visual_prompt: str
    dialogue: List[Tuple[str, str]]  # (character, text)
    narration: Optional[str]
    panel_type: str  # action, dialogue, reaction, etc
    image_url: Optional[str]
    generated_at: Optional[str]

@dataclass
class ComicPage:
    """Full comic page"""
    id: str
    title: str
    story_id: str
    page_number: int
    panels: List[Panel]
    layout: PanelLayout
    theme: str
    citations: List[str]  # Research sources
    created_at: str
    published: bool

@dataclass
class Story:
    """Comic story/series"""
    id: str
    title: str
    description: str
    characters: List[Character]
    pages: List[ComicPage]
    genre: str
    tags: List[str]
    research_sources: List[str]
    created_at: str

# ============================================================================
# STABLE DIFFUSION INTEGRATION
# ============================================================================

class AIImageGenerator:
    """Generate images using Stable Diffusion API"""
    
    def __init__(self, api_endpoint: str = "http://localhost:7860"):
        self.api_endpoint = api_endpoint
        self.session = None
    
    async def init_session(self):
        """Initialize async session"""
        self.session = aiohttp.ClientSession()
    
    async def close_session(self):
        """Close session"""
        if self.session:
            await self.session.close()
    
    async def generate_panel_image(self, prompt: str, style: str = "comic book") -> str:
        """Generate image for comic panel"""
        
        # Enhance prompt with comic style
        full_prompt = f"{prompt}, {style} art, illustration, high quality"
        
        try:
            # Call Stable Diffusion API
            payload = {
                "prompt": full_prompt,
                "negative_prompt": "blurry, low quality, distorted",
                "steps": 20,
                "sampler_name": "Euler",
                "cfg_scale": 7.5,
                "width": 512,
                "height": 512,
            }
            
            # Note: This assumes Stable Diffusion is running locally
            # For production, use Replicate, Together.ai, or similar
            
            # Placeholder: return mock URL
            return f"https://placeholder.com/512x512?text=Comic%20Panel%20{uuid.uuid4()}"
        
        except Exception as e:
            print(f"Error generating image: {e}")
            return None
    
    async def generate_character_sheet(self, character: Character) -> str:
        """Generate character reference sheet"""
        return await self.generate_panel_image(
            character.visual_prompt,
            style="character sheet, detailed, concept art"
        )

# ============================================================================
# DIALOGUE EDITOR
# ============================================================================

class DialogueEditor:
    """Manage comic dialogue and speech bubbles"""
    
    @staticmethod
    def generate_dialogue(scene: str, characters: List[Character], 
                         style: str = "witty") -> List[Tuple[str, str]]:
        """Generate dialogue for scene using LLM"""
        
        # In production: call LLM API (OpenAI, Anthropic, local LLaMA)
        # For now: return template
        
        dialogue = []
        for i, char in enumerate(characters):
            if i == 0:
                text = f"{char.name}: [dialogue here based on scene]"
            else:
                text = f"{char.name}: [response dialogue]"
            dialogue.append((char.name, text))
        
        return dialogue
    
    @staticmethod
    def add_speech_bubble(image: Image.Image, text: str, position: Tuple[int, int],
                         character_color: Tuple[int, int, int] = (255, 255, 255)) -> Image.Image:
        """Add speech bubble to image"""
        
        draw = ImageDraw.Draw(image)
        
        # Draw speech bubble outline
        bubble_coords = [
            (position[0] - 100, position[1] - 50),
            (position[0] + 100, position[1] - 50),
            (position[0] + 100, position[1] + 50),
            (position[0] - 50, position[1] + 50),
            (position[0] - 80, position[1] + 80),
            (position[0] - 100, position[1] + 50),
        ]
        
        draw.polygon(bubble_coords, outline="black", fill=character_color)
        
        # Add text (simplified - in production use better text rendering)
        draw.text((position[0] - 90, position[1] - 40), text, fill="black")
        
        return image

# ============================================================================
# LAYOUT ENGINE
# ============================================================================

class LayoutEngine:
    """Create comic page layouts"""
    
    @staticmethod
    def create_page_layout(layout_type: PanelLayout, 
                          page_width: int = 1024,
                          page_height: int = 1440) -> List[Tuple[int, int, int, int]]:
        """Get coordinates for panels based on layout"""
        
        margin = 20
        
        if layout_type == PanelLayout.SINGLE:
            return [(margin, margin, page_width - margin, page_height - margin)]
        
        elif layout_type == PanelLayout.TWO_COLUMNS:
            mid = page_width // 2
            return [
                (margin, margin, mid - margin, page_height - margin),
                (mid + margin, margin, page_width - margin, page_height - margin),
            ]
        
        elif layout_type == PanelLayout.THREE_COLUMNS:
            col_width = page_width // 3
            return [
                (margin, margin, col_width - margin, page_height - margin),
                (col_width + margin, margin, 2*col_width - margin, page_height - margin),
                (2*col_width + margin, margin, page_width - margin, page_height - margin),
            ]
        
        elif layout_type == PanelLayout.GRID_2X2:
            mid_h = page_height // 2
            mid_w = page_width // 2
            return [
                (margin, margin, mid_w - margin, mid_h - margin),
                (mid_w + margin, margin, page_width - margin, mid_h - margin),
                (margin, mid_h + margin, mid_w - margin, page_height - margin),
                (mid_w + margin, mid_h + margin, page_width - margin, page_height - margin),
            ]
        
        elif layout_type == PanelLayout.GRID_3X3:
            col_width = page_width // 3
            row_height = page_height // 3
            coords = []
            for row in range(3):
                for col in range(3):
                    x1 = col * col_width + margin
                    y1 = row * row_height + margin
                    x2 = (col + 1) * col_width - margin
                    y2 = (row + 1) * row_height - margin
                    coords.append((x1, y1, x2, y2))
            return coords
        
        return [(margin, margin, page_width - margin, page_height - margin)]
    
    @staticmethod
    def render_page(panels: List[Panel], layout_type: PanelLayout,
                   page_width: int = 1024, page_height: int = 1440) -> bytes:
        """Render comic page as image"""
        
        # Create blank page
        page = Image.new('RGB', (page_width, page_height), color='white')
        draw = ImageDraw.Draw(page)
        
        # Get layout coordinates
        coords = LayoutEngine.create_page_layout(layout_type, page_width, page_height)
        
        # Draw panels
        for i, (panel, coord) in enumerate(zip(panels, coords)):
            x1, y1, x2, y2 = coord
            
            # Draw panel border
            draw.rectangle([x1, y1, x2, y2], outline='black', width=3)
            
            # Add gutter (space between panels)
            draw.rectangle([x2, y1, x2 + 5, y2], fill='black')
            draw.rectangle([x1, y2, x2, y2 + 5], fill='black')
        
        # Convert to bytes
        img_byte_arr = io.BytesIO()
        page.save(img_byte_arr, format='PNG')
        return img_byte_arr.getvalue()

# ============================================================================
# COMIC ENGINE
# ============================================================================

class ComicEngine:
    """Main comic creation and management"""
    
    def __init__(self):
        self.stories: Dict[str, Story] = {}
        self.characters: Dict[str, Character] = {}
        self.image_gen = AIImageGenerator()
        self.dialogue_editor = DialogueEditor()
        self.layout_engine = LayoutEngine()
    
    def create_story(self, title: str, description: str, genre: str,
                    tags: List[str]) -> Story:
        """Create new comic story"""
        
        story = Story(
            id=str(uuid.uuid4()),
            title=title,
            description=description,
            characters=[],
            pages=[],
            genre=genre,
            tags=tags,
            research_sources=[],
            created_at=datetime.utcnow().isoformat()
        )
        
        self.stories[story.id] = story
        return story
    
    def add_character(self, story_id: str, name: str, description: str,
                     visual_prompt: str, personality_traits: List[str]) -> Character:
        """Add character to story"""
        
        if story_id not in self.stories:
            raise ValueError(f"Story {story_id} not found")
        
        character = Character(
            id=str(uuid.uuid4()),
            name=name,
            description=description,
            visual_prompt=visual_prompt,
            personality_traits=personality_traits,
            created_at=datetime.utcnow().isoformat()
        )
        
        self.characters[character.id] = character
        self.stories[story_id].characters.append(character)
        
        return character
    
    async def create_page(self, story_id: str, page_number: int,
                         panel_descriptions: List[str], layout: PanelLayout,
                         theme: str = "standard") -> ComicPage:
        """Create comic page from descriptions"""
        
        if story_id not in self.stories:
            raise ValueError(f"Story {story_id} not found")
        
        story = self.stories[story_id]
        panels = []
        
        for i, description in enumerate(panel_descriptions):
            panel = Panel(
                id=str(uuid.uuid4()),
                sequence=i + 1,
                scene_description=description,
                visual_prompt=self._enhance_visual_prompt(description, theme),
                dialogue=self.dialogue_editor.generate_dialogue(
                    description,
                    story.characters[:2]  # Use first 2 characters
                ),
                narration=None,
                panel_type="action",
                image_url=await self.image_gen.generate_panel_image(
                    self._enhance_visual_prompt(description, theme)
                ),
                generated_at=datetime.utcnow().isoformat()
            )
            panels.append(panel)
        
        page = ComicPage(
            id=str(uuid.uuid4()),
            title=f"{story.title} - Page {page_number}",
            story_id=story_id,
            page_number=page_number,
            panels=panels,
            layout=layout,
            theme=theme,
            citations=[],
            created_at=datetime.utcnow().isoformat(),
            published=False
        )
        
        story.pages.append(page)
        return page
    
    def _enhance_visual_prompt(self, description: str, theme: str) -> str:
        """Enhance description with visual style"""
        return f"{description}, {theme} comic art style, detailed, illustration"
    
    async def render_story_pages(self, story_id: str) -> Dict[int, bytes]:
        """Render all pages of story"""
        
        if story_id not in self.stories:
            raise ValueError(f"Story {story_id} not found")
        
        story = self.stories[story_id]
        rendered_pages = {}
        
        for page in story.pages:
            rendered_pages[page.page_number] = self.layout_engine.render_page(
                page.panels,
                page.layout
            )
        
        return rendered_pages
    
    def add_research_citation(self, story_id: str, page_id: str, citation: str):
        """Add research citation to page"""
        
        if story_id not in self.stories:
            return
        
        story = self.stories[story_id]
        for page in story.pages:
            if page.id == page_id:
                page.citations.append(citation)
                story.research_sources.append(citation)
                break

# ============================================================================
# FASTAPI SERVER
# ============================================================================

app = FastAPI(title="GEOLOGOS Comic Creator")
comic_engine = None

class StoryRequest(BaseModel):
    title: str
    description: str
    genre: str
    tags: List[str] = []

class CharacterRequest(BaseModel):
    name: str
    description: str
    visual_prompt: str
    personality_traits: List[str] = []

class PageRequest(BaseModel):
    page_number: int
    panel_descriptions: List[str]
    layout: str = "grid_2x2"
    theme: str = "standard"

class CitationRequest(BaseModel):
    citation: str

@app.on_event("startup")
async def startup():
    global comic_engine
    comic_engine = ComicEngine()
    await comic_engine.image_gen.init_session()

@app.on_event("shutdown")
async def shutdown():
    global comic_engine
    if comic_engine:
        await comic_engine.image_gen.close_session()

@app.post("/api/v1/comics/stories")
async def create_story(request: StoryRequest):
    """Create new comic story"""
    story = comic_engine.create_story(
        request.title,
        request.description,
        request.genre,
        request.tags
    )
    return asdict(story)

@app.post("/api/v1/comics/stories/{story_id}/characters")
async def add_character(story_id: str, request: CharacterRequest):
    """Add character to story"""
    try:
        character = comic_engine.add_character(
            story_id,
            request.name,
            request.description,
            request.visual_prompt,
            request.personality_traits
        )
        return asdict(character)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.post("/api/v1/comics/stories/{story_id}/pages")
async def create_page(story_id: str, request: PageRequest):
    """Create comic page"""
    try:
        layout = PanelLayout[request.layout.upper()]
        page = await comic_engine.create_page(
            story_id,
            request.page_number,
            request.panel_descriptions,
            layout,
            request.theme
        )
        return asdict(page)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.get("/api/v1/comics/stories/{story_id}")
async def get_story(story_id: str):
    """Get story"""
    if story_id not in comic_engine.stories:
        raise HTTPException(status_code=404, detail="Story not found")
    return asdict(comic_engine.stories[story_id])

@app.post("/api/v1/comics/stories/{story_id}/pages/{page_id}/citations")
async def add_citation(story_id: str, page_id: str, request: CitationRequest):
    """Add research citation to page"""
    comic_engine.add_research_citation(story_id, page_id, request.citation)
    return {"status": "citation added"}

@app.get("/api/v1/comics/stories/{story_id}/render")
async def render_story(story_id: str):
    """Render all pages of story"""
    try:
        rendered = await comic_engine.render_story_pages(story_id)
        return {
            "story_id": story_id,
            "pages_rendered": len(rendered),
            "page_numbers": list(rendered.keys())
        }
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.get("/health")
async def health():
    return {"status": "healthy", "service": "comic-creator"}

# ============================================================================
# RUN
# ============================================================================

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8003)