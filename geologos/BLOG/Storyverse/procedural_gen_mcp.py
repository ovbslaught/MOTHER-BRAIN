import asyncio
import json
import random
from typing import Dict, List
from mcp.server import Server
from mcp.types import Tool, TextContent
from ollama import AsyncClient
from datetime import datetime
import os
import yaml

app = Server("procedural-generation")

VAULT_DIR = os.getenv('OBSIDIAN_VAULT_PATH')
ollama_client = AsyncClient(host=os.getenv('OLLAMA_HOST'))
config = yaml.safe_load(open('config.yaml'))

@app.list_tools()
async def list_tools() -> List[Tool]:
    return [
        Tool(name="generate_character", description="Generate a character", inputSchema={"type": "object", "properties": {"name": {"type": "string"}}}),
        Tool(name="generate_location", description="Generate a location", inputSchema={"type": "object", "properties": {"name": {"type": "string"}}}),
        Tool(name="generate_quest", description="Generate a quest", inputSchema={"type": "object", "properties": {"title": {"type": "string"}}}),
        Tool(name="generate_item", description="Generate an item", inputSchema={"type": "object", "properties": {"name": {"type": "string"}}}),
        Tool(name="generate_faction", description="Generate a faction", inputSchema={"type": "object", "properties": {"name": {"type": "string"}}}),
    ]

@app.call_tool()
async def call_tool(name: str, arguments: Dict) -> List[TextContent]:
    if name.startswith("generate_"):
        element_type = name.split("_")[1]
        data = await generate_entity(element_type, arguments)
        # Save to vault (direct for simplicity; in full, use obsidian MCP)
        entity_dir = os.path.join(VAULT_DIR, element_type.capitalize() + "s")
        os.makedirs(entity_dir, exist_ok=True)
        file_name = data["name"].replace(" ", "_") + ".md"
        with open(os.path.join(entity_dir, file_name), 'w') as f:
            f.write("---\n")
            yaml.dump(data, f)
            f.write("---\n\n" + data.get("description", ""))
        return [TextContent(type="text", text=data)]

async def generate_entity(element_type: str, args: Dict) -> Dict:
    name = args.get("name", f"{element_type.capitalize()} {random.randint(1,1000)}")
    prompt = f"Generate {element_type}: Name {name}. Follow schema from report."
    description = await call_ollama(prompt)
    data = {
        "type": element_type,
        "name": name,
        "ai_generated": True,
        "created": datetime.utcnow().isoformat(),
        "last_updated": datetime.utcnow().isoformat(),
    }
    # Add type-specific attributes based on schemas
    if element_type == "character":
        data.update({
            "archetype": random.choice(["hero", "villain", "mentor"]),
            "faction": "neutral",
            "status": "alive",
            "location": "unknown",
            "stats": {stat: random.randint(1,18) for stat in ["strength", "intelligence", "charisma", "dexterity", "constitution", "wisdom"]},
            "relationships": [],
            "traits": ["brave", "wise"],
            "inventory": [],
            "quests": [],
            "level": random.randint(1,100),
            "health": 100,
            "max_health": 100,
        })
    # Similarly for location, quest, item, faction (implement full as per schemas)
    data["description"] = description
    return data

async def call_ollama(prompt: str) -> str:
    response = await ollama_client.generate(model="mistral:7b-instruct", prompt=prompt, options={"temperature": 0.8})
    return response['response']

async def main():
    from mcp.server.stdio import stdio_server
    async with stdio_server() as (read, write):
        await app.run(read, write, app.create_initialization_options())

if __name__ == "__main__":
    asyncio.run(main())