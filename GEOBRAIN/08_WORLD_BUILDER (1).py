#!/usr/bin/env python3
"""
GEOLOGOS WORLD BUILDER: Production Ready
Procedural world generation + location database + map visualization
Deploy: python world_builder.py
"""

import json
import uuid
import random
import math
from datetime import datetime
from typing import List, Dict, Optional, Tuple
from dataclasses import dataclass, asdict
from enum import Enum

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

# ============================================================================
# DATA STRUCTURES
# ============================================================================

class BiomeType(Enum):
    """Biome types"""
    FOREST = "forest"
    DESERT = "desert"
    MOUNTAIN = "mountain"
    OCEAN = "ocean"
    TUNDRA = "tundra"
    GRASSLAND = "grassland"
    SWAMP = "swamp"
    URBAN = "urban"
    VOID = "void"  # For Nomadz universe

class LocationType(Enum):
    """Location types"""
    CITY = "city"
    VILLAGE = "village"
    DUNGEON = "dungeon"
    SHRINE = "shrine"
    RESEARCH_CENTER = "research_center"
    OUTPOST = "outpost"
    RUIN = "ruin"
    NATURAL_LANDMARK = "natural_landmark"
    DIMENSIONAL_PORTAL = "dimensional_portal"

@dataclass
class Coordinates:
    """2D/3D coordinates"""
    x: float
    y: float
    z: Optional[float] = None

@dataclass
class Location:
    """Single location/place"""
    id: str
    name: str
    location_type: LocationType
    coordinates: Coordinates
    biome: BiomeType
    description: str
    population: int
    ruler: Optional[str]
    resources: List[str]
    connections: List[str]  # Connected location IDs
    discovered: bool
    significance: str  # Why it matters in story
    lore: str
    created_at: str

@dataclass
class Region:
    """Geographic region"""
    id: str
    name: str
    biome: BiomeType
    locations: List[str]  # location IDs
    area_sq_km: float
    history: str
    cultures: List[str]
    created_at: str

@dataclass
class World:
    """Complete world"""
    id: str
    name: str
    universe: str  # "nomadz", "earth", etc
    dimensions: Tuple[float, float, float]  # x, y, z extent
    locations: Dict[str, Location]
    regions: Dict[str, Region]
    biome_map: List[List[BiomeType]]  # Grid of biomes
    lore: str
    creation_date_in_story: Optional[str]
    discovered_date: str
    citations: List[str]
    created_at: str

# ============================================================================
# WORLD GENERATOR
# ============================================================================

class WorldBuilder:
    """Build and manage worlds"""
    
    def __init__(self):
        self.worlds: Dict[str, World] = {}
        self.noise_cache = {}
    
    def create_world(self, name: str, universe: str, width: int = 100,
                    height: int = 100) -> World:
        """Create new world"""
        
        world = World(
            id=str(uuid.uuid4()),
            name=name,
            universe=universe,
            dimensions=(float(width), float(height), 100.0),
            locations={},
            regions={},
            biome_map=self._generate_biome_map(width, height),
            lore="",
            creation_date_in_story=None,
            discovered_date=datetime.utcnow().isoformat(),
            citations=[],
            created_at=datetime.utcnow().isoformat()
        )
        
        self.worlds[world.id] = world
        return world
    
    def _generate_biome_map(self, width: int, height: int) -> List[List[BiomeType]]:
        """Generate random biome map using noise"""
        
        biome_map = []
        biome_list = list(BiomeType)
        
        for y in range(height):
            row = []
            for x in range(width):
                # Simple random biome assignment (in production: use Perlin noise)
                biome = random.choice(biome_list)
                row.append(biome)
            biome_map.append(row)
        
        return biome_map
    
    def add_location(self, world_id: str, name: str, location_type: LocationType,
                    x: float, y: float, z: float = 0,
                    biome: Optional[BiomeType] = None,
                    description: str = "") -> Location:
        """Add location to world"""
        
        if world_id not in self.worlds:
            raise ValueError(f"World {world_id} not found")
        
        world = self.worlds[world_id]
        location_id = str(uuid.uuid4())
        
        # Auto-detect biome from map if not specified
        if biome is None:
            map_x = int(x) % len(world.biome_map[0])
            map_y = int(y) % len(world.biome_map)
            biome = world.biome_map[map_y][map_x]
        
        location = Location(
            id=location_id,
            name=name,
            location_type=location_type,
            coordinates=Coordinates(x, y, z),
            biome=biome,
            description=description,
            population=random.randint(0, 100000),
            ruler=None,
            resources=self._get_biome_resources(biome),
            connections=[],
            discovered=False,
            significance="",
            lore="",
            created_at=datetime.utcnow().isoformat()
        )
        
        world.locations[location_id] = location
        return location
    
    def _get_biome_resources(self, biome: BiomeType) -> List[str]:
        """Get resources for biome"""
        
        resources = {
            BiomeType.FOREST: ["wood", "herbs", "game"],
            BiomeType.DESERT: ["sand", "minerals", "energy"],
            BiomeType.MOUNTAIN: ["ore", "stone", "gems"],
            BiomeType.OCEAN: ["fish", "pearls", "salt"],
            BiomeType.TUNDRA: ["ice", "furs", "rare crystals"],
            BiomeType.GRASSLAND: ["wheat", "livestock", "honey"],
            BiomeType.SWAMP: ["rare plants", "alligator hides", "marsh gas"],
            BiomeType.URBAN: ["technology", "artifacts", "knowledge"],
            BiomeType.VOID: ["cosmic dust", "dark matter", "quantum energy"],
        }
        
        return resources.get(biome, [])
    
    def connect_locations(self, world_id: str, location_a_id: str,
                         location_b_id: str):
        """Connect two locations (trade route, path, etc)"""
        
        if world_id not in self.worlds:
            return
        
        world = self.worlds[world_id]
        
        if location_a_id in world.locations and location_b_id in world.locations:
            world.locations[location_a_id].connections.append(location_b_id)
            world.locations[location_b_id].connections.append(location_a_id)
    
    def create_region(self, world_id: str, name: str, biome: BiomeType,
                     area_sq_km: float, history: str = "") -> Region:
        """Create region"""
        
        if world_id not in self.worlds:
            raise ValueError(f"World {world_id} not found")
        
        region = Region(
            id=str(uuid.uuid4()),
            name=name,
            biome=biome,
            locations=[],
            area_sq_km=area_sq_km,
            history=history,
            cultures=[],
            created_at=datetime.utcnow().isoformat()
        )
        
        self.worlds[world_id].regions[region.id] = region
        return region
    
    def add_location_to_region(self, world_id: str, region_id: str,
                              location_id: str):
        """Add location to region"""
        
        if world_id not in self.worlds:
            return
        
        world = self.worlds[world_id]
        
        if region_id in world.regions and location_id in world.locations:
            world.regions[region_id].locations.append(location_id)
    
    def discover_location(self, world_id: str, location_id: str,
                         discovered_by: Optional[str] = None):
        """Mark location as discovered"""
        
        if world_id not in self.worlds:
            return
        
        world = self.worlds[world_id]
        
        if location_id in world.locations:
            world.locations[location_id].discovered = True
    
    def get_shortest_path(self, world_id: str, start_location_id: str,
                         end_location_id: str) -> Optional[List[str]]:
        """Find shortest path between locations"""
        
        if world_id not in self.worlds:
            return None
        
        world = self.worlds[world_id]
        
        # Simple BFS
        from collections import deque
        
        queue = deque([(start_location_id, [start_location_id])])
        visited = {start_location_id}
        
        while queue:
            current, path = queue.popleft()
            
            if current == end_location_id:
                return path
            
            if current in world.locations:
                for neighbor in world.locations[current].connections:
                    if neighbor not in visited:
                        visited.add(neighbor)
                        queue.append((neighbor, path + [neighbor]))
        
        return None
    
    def generate_map_visualization(self, world_id: str) -> str:
        """Generate ASCII map visualization"""
        
        if world_id not in self.worlds:
            return ""
        
        world = self.worlds[world_id]
        
        if not world.biome_map:
            return "No map data"
        
        biome_symbols = {
            BiomeType.FOREST: "🌲",
            BiomeType.DESERT: "🏜️",
            BiomeType.MOUNTAIN: "⛰️",
            BiomeType.OCEAN: "🌊",
            BiomeType.TUNDRA: "❄️",
            BiomeType.GRASSLAND: "🌾",
            BiomeType.SWAMP: "🦗",
            BiomeType.URBAN: "🏙️",
            BiomeType.VOID: "🌌",
        }
        
        map_str = f"# Map of {world.name}\n\n"
        
        for row in world.biome_map[:20]:  # Limit to 20 rows for display
            for biome in row[:30]:  # Limit to 30 cols
                map_str += biome_symbols.get(biome, "?")
            map_str += "\n"
        
        # Add locations
        map_str += "\n## Locations\n"
        for loc_id, location in world.locations.items():
            map_str += f"- {location.name} ({location.location_type.value}): {location.description[:50]}...\n"
        
        return map_str

# ============================================================================
# FASTAPI SERVER
# ============================================================================

app = FastAPI(title="GEOLOGOS World Builder")
builder = None

class WorldRequest(BaseModel):
    name: str
    universe: str
    width: int = 100
    height: int = 100

class LocationRequest(BaseModel):
    name: str
    location_type: str
    x: float
    y: float
    z: float = 0
    biome: Optional[str] = None
    description: str = ""

class RegionRequest(BaseModel):
    name: str
    biome: str
    area_sq_km: float
    history: str = ""

class ConnectionRequest(BaseModel):
    location_a_id: str
    location_b_id: str

@app.on_event("startup")
async def startup():
    global builder
    builder = WorldBuilder()

@app.post("/api/v1/worlds")
async def create_world(request: WorldRequest):
    """Create new world"""
    world = builder.create_world(request.name, request.universe,
                                 request.width, request.height)
    return asdict(world)

@app.post("/api/v1/worlds/{world_id}/locations")
async def add_location(world_id: str, request: LocationRequest):
    """Add location to world"""
    try:
        location_type = LocationType[request.location_type.upper()]
        biome = BiomeType[request.biome.upper()] if request.biome else None
        
        location = builder.add_location(
            world_id,
            request.name,
            location_type,
            request.x,
            request.y,
            request.z,
            biome,
            request.description
        )
        return asdict(location)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.post("/api/v1/worlds/{world_id}/regions")
async def create_region(world_id: str, request: RegionRequest):
    """Create region"""
    try:
        biome = BiomeType[request.biome.upper()]
        region = builder.create_region(
            world_id,
            request.name,
            biome,
            request.area_sq_km,
            request.history
        )
        return asdict(region)
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.post("/api/v1/worlds/{world_id}/connections")
async def add_connection(world_id: str, request: ConnectionRequest):
    """Connect locations"""
    builder.connect_locations(
        world_id,
        request.location_a_id,
        request.location_b_id
    )
    return {"status": "connected"}

@app.get("/api/v1/worlds/{world_id}")
async def get_world(world_id: str):
    """Get world"""
    if world_id not in builder.worlds:
        raise HTTPException(status_code=404, detail="World not found")
    return asdict(builder.worlds[world_id])

@app.get("/api/v1/worlds/{world_id}/map")
async def get_map(world_id: str):
    """Get map visualization"""
    map_vis = builder.generate_map_visualization(world_id)
    return {"map": map_vis}

@app.get("/api/v1/worlds/{world_id}/locations/{location_id}")
async def get_location(world_id: str, location_id: str):
    """Get location"""
    if world_id not in builder.worlds:
        raise HTTPException(status_code=404, detail="World not found")
    
    world = builder.worlds[world_id]
    if location_id not in world.locations:
        raise HTTPException(status_code=404, detail="Location not found")
    
    return asdict(world.locations[location_id])

@app.get("/health")
async def health():
    return {"status": "healthy", "service": "world-builder"}

# ============================================================================
# RUN
# ============================================================================

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8007)