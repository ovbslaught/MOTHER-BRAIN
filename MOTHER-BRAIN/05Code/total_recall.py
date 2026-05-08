#!/usr/bin/env python3
import os
import sys

VAULT_PATH = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/Obsidian_NOMADZ_Archive"

def search_vault(query):
    print(f"cat> [RECALL] Searching for: '{query}'...")
    results = []
    
    # Walking through all sub-directories (Characters, Technical, Story)
    for root, dirs, files in os.walk(VAULT_PATH):
        for file in files:
            if file.endswith(".md"):
                full_path = os.path.join(root, file)
                with open(full_path, 'r') as f:
                    content = f.read()
                    if query.lower() in content.lower():
                        results.append(file)
    
    if results:
        print(f"cat> [RECALL] Matches found in: {', '.join(results)}")
    else:
        print("cat> [RECALL] Zero fragments found. Adjusting search parameters...")

if __name__ == "__main__":
    if len(sys.argv) > 1:
        search_vault(" ".join(sys.argv[1:]))
    else:
        print("cat> [ERROR] No query provided for Total Recall.")
