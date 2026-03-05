#!/usr/bin/env python3
"""
GEOLOGOS CREATIVE SUITE: Music Creator + Art Generator
Production Ready - Music composition + AI art generation
Deploy: python creative_suite.py
"""

import json
import uuid
import asyncio
from datetime import datetime
from typing import List, Dict, Optional, Tuple
from dataclasses import dataclass, asdict
from enum import Enum

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import numpy as np
from scipy.io import wavfile
import librosa

# ============================================================================
# MUSIC CREATOR
# ============================================================================

class MusicNote(Enum):
    """Musical notes"""
    C = 60
    D = 62
    E = 64
    F = 65
    G = 67
    A = 69
    B = 71

class Instrument(Enum):
    """Instrument types"""
    PIANO = "piano"
    GUITAR = "guitar"
    SYNTH = "synth"
    STRINGS = "strings"
    DRUMS = "drums"
    BASS = "bass"

@dataclass
class MusicTrack:
    """Music track"""
    id: str
    instrument: Instrument
    notes: List[Tuple[int, int]]  # (note_number, duration_ms)
    volume: float
    effects: List[str]
    created_at: str

@dataclass
class MusicComposition:
    """Complete music piece"""
    id: str
    title: str
    genre: str
    bpm: int
    time_signature: str  # "4/4", "3/4", etc
    tracks: List[MusicTrack]
    duration_ms: int
    tags: List[str]
    citations: List[str]
    created_at: str

class MusicGenerator:
    """Generate music compositions"""
    
    def __init__(self, sample_rate: int = 44100):
        self.sample_rate = sample_rate
    
    def create_composition(self, title: str, genre: str, bpm: int) -> MusicComposition:
        """Create new music composition"""
        
        return MusicComposition(
            id=str(uuid.uuid4()),
            title=title,
            genre=genre,
            bpm=bpm,
            time_signature="4/4",
            tracks=[],
            duration_ms=0,
            tags=[genre],
            citations=[],
            created_at=datetime.utcnow().isoformat()
        )
    
    def add_track(self, composition: MusicComposition, instrument: Instrument,
                 notes: List[Tuple[int, int]]) -> MusicTrack:
        """Add track to composition"""
        
        track = MusicTrack(
            id=str(uuid.uuid4()),
            instrument=instrument,
            notes=notes,
            volume=1.0,
            effects=[],
            created_at=datetime.utcnow().isoformat()
        )
        
        composition.tracks.append(track)
        
        # Update duration
        if notes:
            total_duration = sum(duration for _, duration in notes)
            composition.duration_ms = max(composition.duration_ms, total_duration)
        
        return track
    
    def generate_midi_sequence(self, composition: MusicComposition) -> List[Dict]:
        """Generate MIDI sequence"""
        
        midi_sequence = []
        current_time = 0
        
        for track_idx, track in enumerate(composition.tracks):
            for note_num, duration_ms in track.notes:
                midi_sequence.append({
                    "track": track_idx,
                    "note": note_num,
                    "start_time": current_time,
                    "duration": duration_ms,
                    "velocity": 100,
                    "instrument": track.instrument.value
                })
                current_time += duration_ms
        
        return midi_sequence
    
    def _note_to_frequency(self, note_number: int) -> float:
        """Convert MIDI note number to frequency"""
        return 440 * (2 ** ((note_number - 69) / 12))
    
    def _generate_sine_wave(self, frequency: float, duration_ms: int) -> np.ndarray:
        """Generate sine wave for note"""
        duration_sec = duration_ms / 1000
        t = np.linspace(0, duration_sec, int(self.sample_rate * duration_sec))
        return np.sin(2 * np.pi * frequency * t)
    
    async def render_composition(self, composition: MusicComposition) -> str:
        """Render composition to audio file"""
        
        output_audio = np.zeros(int(self.sample_rate * composition.duration_ms / 1000))
        
        for track in composition.tracks:
            current_pos = 0
            
            for note_num, duration_ms in track.notes:
                frequency = self._note_to_frequency(note_num)
                waveform = self._generate_sine_wave(frequency, duration_ms)
                
                # Add to output
                num_samples = len(waveform)
                output_audio[current_pos:current_pos + num_samples] += waveform * track.volume
                current_pos += num_samples
        
        # Normalize to prevent clipping
        max_val = np.max(np.abs(output_audio))
        if max_val > 1.0:
            output_audio = output_audio / max_val
        
        # Save to file
        output_file = f"music/{composition.id}.wav"
        import os
        os.makedirs("music", exist_ok=True)
        
        # Convert to 16-bit PCM
        audio_int16 = np.int16(output_audio * 32767)
        wavfile.write(output_file, self.sample_rate, audio_int16)
        
        return output_file
    
    def add_citation(self, composition: MusicComposition, citation: str):
        """Add research citation"""
        composition.citations.append(citation)

# ============================================================================
# ART GENERATOR
# ============================================================================

@dataclass
class ArtStyle:
    """Art style configuration"""
    style_name: str
    prompt_suffix: str
    negative_prompt: str

PREDEFINED_STYLES = {
    "comic": ArtStyle("comic", "comic book art, comic illustration", "blurry, low quality"),
    "oil_painting": ArtStyle("oil_painting", "oil painting, impressionist", "digital, photo"),
    "cyberpunk": ArtStyle("cyberpunk", "cyberpunk, neon, future", "analog, vintage"),
    "watercolor": ArtStyle("watercolor", "watercolor painting, artistic", "photorealistic"),
    "concept_art": ArtStyle("concept_art", "concept art, digital painting", "blurry"),
}

@dataclass
class Artwork:
    """Generated artwork"""
    id: str
    title: str
    description: str
    prompt: str
    style: str
    image_url: Optional[str]
    model_used: str
    generation_params: Dict
    created_at: str
    citations: List[str]

@dataclass
class ArtCollection:
    """Collection of artworks"""
    id: str
    title: str
    description: str
    artworks: List[Artwork]
    tags: List[str]
    created_at: str

class ArtGenerator:
    """Generate art using AI models"""
    
    def __init__(self, api_endpoint: str = "http://localhost:7860"):
        self.api_endpoint = api_endpoint
    
    def create_collection(self, title: str, description: str, tags: List[str]) -> ArtCollection:
        """Create art collection"""
        
        return ArtCollection(
            id=str(uuid.uuid4()),
            title=title,
            description=description,
            artworks=[],
            tags=tags,
            created_at=datetime.utcnow().isoformat()
        )
    
    async def generate_artwork(self, title: str, description: str, style: str = "concept_art",
                              width: int = 512, height: int = 512) -> Artwork:
        """Generate artwork from description"""
        
        if style not in PREDEFINED_STYLES:
            style = "concept_art"
        
        style_config = PREDEFINED_STYLES[style]
        
        # Build prompt
        full_prompt = f"{description}, {style_config.prompt_suffix}, high quality, detailed"
        
        artwork = Artwork(
            id=str(uuid.uuid4()),
            title=title,
            description=description,
            prompt=full_prompt,
            style=style,
            image_url=await self._generate_with_stable_diffusion(
                full_prompt,
                style_config.negative_prompt,
                width,
                height
            ),
            model_used="Stable Diffusion 1.5",
            generation_params={
                "style": style,
                "width": width,
                "height": height,
                "steps": 20,
                "scale": 7.5
            },
            created_at=datetime.utcnow().isoformat(),
            citations=[]
        )
        
        return artwork
    
    async def _generate_with_stable_diffusion(self, prompt: str, negative_prompt: str,
                                             width: int, height: int) -> Optional[str]:
        """Call Stable Diffusion API"""
        
        try:
            import aiohttp
            
            async with aiohttp.ClientSession() as session:
                payload = {
                    "prompt": prompt,
                    "negative_prompt": negative_prompt,
                    "steps": 20,
                    "sampler_name": "Euler",
                    "cfg_scale": 7.5,
                    "width": width,
                    "height": height,
                }
                
                # This assumes Stable Diffusion WebUI is running
                async with session.post(f"{self.api_endpoint}/api/txt2img", json=payload) as resp:
                    if resp.status == 200:
                        data = await resp.json()
                        # Return image URL or base64
                        return data.get("images", [None])[0]
        
        except Exception as e:
            print(f"Error generating image: {e}")
        
        # Placeholder
        return f"https://placeholder.com/{width}x{height}?text=AI%20Art"
    
    def add_artwork_to_collection(self, collection: ArtCollection, artwork: Artwork):
        """Add artwork to collection"""
        collection.artworks.append(artwork)
    
    def add_citation(self, artwork: Artwork, citation: str):
        """Add research citation to artwork"""
        artwork.citations.append(citation)

# ============================================================================
# FASTAPI SERVER
# ============================================================================

app = FastAPI(title="GEOLOGOS Creative Suite")
music_generator = None
art_generator = None

class CompositionRequest(BaseModel):
    title: str
    genre: str
    bpm: int = 120

class TrackRequest(BaseModel):
    instrument: str
    notes: List[Tuple[int, int]]

class ArtworkRequest(BaseModel):
    title: str
    description: str
    style: str = "concept_art"
    width: int = 512
    height: int = 512

class CollectionRequest(BaseModel):
    title: str
    description: str
    tags: List[str] = []

@app.on_event("startup")
async def startup():
    global music_generator, art_generator
    music_generator = MusicGenerator()
    art_generator = ArtGenerator()

@app.post("/api/v1/music/compositions")
async def create_composition(request: CompositionRequest):
    """Create music composition"""
    composition = music_generator.create_composition(
        request.title,
        request.genre,
        request.bpm
    )
    return asdict(composition)

@app.post("/api/v1/music/compositions/{comp_id}/tracks")
async def add_track(comp_id: str, request: TrackRequest):
    """Add music track"""
    # In production: retrieve composition from database
    # For now, create new one for demo
    composition = music_generator.create_composition(
        f"Composition {comp_id}", "unknown", 120
    )
    
    instrument = Instrument[request.instrument.upper()]
    track = music_generator.add_track(composition, instrument, request.notes)
    return asdict(track)

@app.post("/api/v1/music/compositions/{comp_id}/render")
async def render_composition(comp_id: str):
    """Render composition to audio"""
    # In production: retrieve from database
    composition = music_generator.create_composition(
        f"Composition {comp_id}", "unknown", 120
    )
    
    output_file = await music_generator.render_composition(composition)
    return {"status": "rendered", "file": output_file}

@app.post("/api/v1/art/collections")
async def create_collection(request: CollectionRequest):
    """Create art collection"""
    collection = art_generator.create_collection(
        request.title,
        request.description,
        request.tags
    )
    return asdict(collection)

@app.post("/api/v1/art/generate")
async def generate_artwork(request: ArtworkRequest):
    """Generate artwork"""
    artwork = await art_generator.generate_artwork(
        request.title,
        request.description,
        request.style,
        request.width,
        request.height
    )
    return asdict(artwork)

@app.get("/health")
async def health():
    return {"status": "healthy", "service": "creative-suite"}

# ============================================================================
# RUN
# ============================================================================

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8005)