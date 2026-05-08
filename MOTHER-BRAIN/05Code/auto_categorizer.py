#!/usr/bin/env python3
import os

PILLAR_MAP = {
    "Pillar_Geometric": ["basalt", "pyramid", "geometry", "non-euclidean"],
    "Pillar_Neural": ["ppo", "neural", "bellman", "stablebaselines3"],
    "Pillar_Decolonial": ["scavenge", "p2p", "substrate", "decolonial"],
    "Pillar_Logic": ["equation", "algorithm", "math", "logic"]
}

VAULT_PATH = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/Obsidian_NOMADZ_Archive/06_CHAT_HISTORY"

def tag_logs():
    for file in os.listdir(VAULT_PATH):
        if file.endswith(".md"):
            with open(os.path.join(VAULT_PATH, file), "r") as f:
                content = f.read().lower()
            
            tags = []
            for pillar, keywords in PILLAR_MAP.items():
                if any(k in content for k in keywords):
                    tags.append(f"#{pillar}")
            
            if tags:
                with open(os.path.join(VAULT_PATH, file), "a") as f:
                    f.write("\n\n## Automated Tags\n" + " ".join(tags))

if __name__ == "__main__":
    tag_logs()
