import os
import shutil
import json
import subprocess
from pathlib import Path

# --- ABSOLUTE PATHS ---
INBOX = Path("/sdcard/Wormhole/MOTHER-BRAIN/00_inbox/TO_PROCESS")
STAGING = Path("/sdcard/Wormhole/MOTHER-BRAIN/01_Knowledge_Graph/Staging")
LOG_PATH = Path("/sdcard/Wormhole/MOTHER-BRAIN/logs/ingestion_log.jsonl")
RCLONE_REMOTE = "WORMHOLE" # Ensure your rclone config matches this

def push_to_drive(local_path, pillar):
    """Pushes to Google Drive and removes local file to recover S23 storage."""
    remote_path = f"{RCLONE_REMOTE}:MOTHER-BRAIN/01_Knowledge_Graph/Staging/{pillar}/"
    try:
        subprocess.run(["rclone", "move", str(local_path), remote_path], check=True)
        return True
    except:
        return False

def process_inbox():
    if not INBOX.exists(): return
    for file in INBOX.iterdir():
        if file.is_file():
            # Pillar Classification
            if file.suffix == ".json": pillar = "PILLAR_04_COMPUTATIONAL"
            elif file.suffix in [".gd", ".tscn", ".gdshader"]: pillar = "PILLAR_02_KINETIC"
            elif file.suffix in [".md", ".txt"]: pillar = "PILLAR_16_LITERATURE"
            else: pillar = "UNCATEGORIZED"
            
            # Local Move -> Cloud Offload
            dest_dir = STAGING / pillar
            dest_dir.mkdir(parents=True, exist_ok=True)
            local_dest = dest_dir / file.name
            shutil.move(str(file), str(local_dest))
            
            sync_success = push_to_drive(local_dest, pillar)
            
            with open(LOG_PATH, "a") as log:
                entry = {"file": file.name, "pillar": pillar, "drive_sync": sync_success, "ts": "2026-03-09"}
                log.write(json.dumps(entry) + "\n")

if __name__ == "__main__":
    process_inbox()
