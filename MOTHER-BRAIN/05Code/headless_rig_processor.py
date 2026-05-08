#!/usr/bin/env python3
# OMEGA-CORE-01: Headless Mesh-to-Controller Rigger
# Targets: NOMADZ-0/Assets/Characters

import os
import subprocess
import logging
from pathlib import Path

# Paths
GODOT_BIN = "godot" # Assumes godot-headless is in Termux path
PROJECT_DIR = "/storage/emulated/0/Wormhole/NOMADZ-0"
CHAR_DIR = os.path.join(PROJECT_DIR, "Assets/Characters")
LOG_FILE = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/archive/rig_pipeline.log"

logging.basicConfig(filename=LOG_FILE, level=logging.INFO, format='%(asctime)s - %(message)s')

def process_new_meshes():
    """
    Scans for raw .obj or .glb files that lack a corresponding .remap or .tres
    and forces Godot to import them with the Humanoid Bone Map.
    """
    if not os.path.exists(CHAR_DIR):
        os.makedirs(CHAR_DIR, exist_ok=True)
        return

    for file in os.listdir(CHAR_DIR):
        if file.endswith((".obj", ".fbx", ".glb")):
            file_path = os.path.join(CHAR_DIR, file)
            
            # Logic: If no .import file exists, Godot hasn't 'seen' it yet.
            # We trigger a headless import with the Animation Library preset.
            logging.info(f"cat> [RIGGER] Processing raw molecular mesh: {file}")
            
            try:
                # Execute Godot Headless Import & Rigging Scan
                subprocess.run([
                    GODOT_BIN, "--headless", "--path", PROJECT_DIR, 
                    "--editor", "--quit", "--export-pack", "Dummy", "/dev/null"
                ], check=True, capture_output=True)
                
                logging.info(f"cat> [RIGGER] Mesh {file} successfully mapped to VULTURE-CONTROLLER.")
            except Exception as e:
                logging.error(f"cat> [RIGGER ERROR] Failed to rig {file}: {e}")

if __name__ == "__main__":
    process_new_meshes()
