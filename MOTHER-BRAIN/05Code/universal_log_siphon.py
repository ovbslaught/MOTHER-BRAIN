#!/usr/bin/env python3
import os
import json

SOURCE_DIR = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/00_Inbox/TO_PROCESS"
VAULT_DIR = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/Obsidian_NOMADZ_Archive/06_CHAT_HISTORY"

os.makedirs(VAULT_DIR, exist_ok=True)

def siphon_chats():
    print("cat> [SIPHON] Scanning for new chat exports...")
    files = [f for f in os.listdir(SOURCE_DIR) if f.endswith((".json", ".txt", ".md"))]
    
    if not files:
        print("cat> [IDLE] No new logs found in TO_PROCESS.")
        return

    for file in files:
        print(f"cat> [LOGGING] Processing: {file}")
        with open(os.path.join(SOURCE_DIR, file), 'r') as f:
            content = f.read()
        
        output_name = f"ARCHIVE_{file.replace('.json', '.md').replace('.txt', '.md')}"
        with open(os.path.join(VAULT_DIR, output_name), 'w') as out:
            out.write(f"# Chat Archive: {file}\n\n{content}\n\n---\n#archive #nomadz #pillar_sync")
        
    print("cat> [SIPHON] All detected logs have been indexed.")

if __name__ == "__main__":
    siphon_chats()
