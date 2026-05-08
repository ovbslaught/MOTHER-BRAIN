#!/usr/bin/env python3
import os

VAULT_PATH = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/Obsidian_NOMADZ_Archive"

lore_sections = {
    "The_Signal_Sigma": "The narrative universe bridging the 757-Sigma to the Atlantis-Cluster. A high-frequency data substrate.",
    "Geo-Logos_AGI": "The bridge between biological and digital consciousness. The core 'Mother-Brain' protocol.",
    "Vulture_Legion": "The automated drone fleet used for P2P resource scavenging and Dyson-ring assembly.",
    "The_Resonance_War": "The conflict between NOMADZ resistance and the centralized Signal-Dampeners."
}

def write_lore():
    for title, content in lore_sections.items():
        path = f"{VAULT_PATH}/02_STORY_BRANCHES/{title}.md"
        os.makedirs(os.path.dirname(path), exist_ok=True)
        with open(path, "w") as f:
            f.write(f"# {title.replace('_', ' ')}\n\n{content}\n\n---\n[[00_INDEX]]")

if __name__ == "__main__":
    write_lore()
