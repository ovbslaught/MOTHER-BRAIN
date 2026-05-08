#!/usr/bin/env python3
import os
import json
from datetime import datetime

VAULT_PATH = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/Obsidian_NOMADZ_Archive"

def create_md(path, title, content):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "w") as f:
        f.write(f"# {title}\n\n{content}\n\n---\n*Last Updated: {datetime.now()}*")

def build_index(data):
    index_content = "## The NOMADZ Meta-Saga Index\n\n"
    for category, items in data.items():
        index_content += f"### {category}\n"
        for item in items:
            index_content += f"- [[{item}]]\n"
    create_md(f"{VAULT_PATH}/00_INDEX.md", "Master Index", index_content)

# Initializing with known lore
lore_map = {
    "Characters": ["Cope", "Proxy", "Echo", "Bytez", "Node", "Spiff", "Shell", "Rian", "Ren"],
    "Story Arcs": ["Descent_to_Earth_Center", "Inertia_SP043", "Project_Neptune", "The_Eye_War"],
    "Technical": ["Vulture_Code", "Signal_Sigma_Runes", "GDScript_Bridges"]
}

if __name__ == "__main__":
    print("cat> [LIBRARIAN] Formatting the Obsidian Book...")
    build_index(lore_map)
    # Create individual character stubs
    for char in lore_map["Characters"]:
        create_md(f"{VAULT_PATH}/01_CHARACTERS/{char}.md", char, f"Lore and abilities for {char} siphoned from Vulture-Brain.")
    print(f"cat> [LIBRARIAN] Archive created at {VAULT_PATH}")
