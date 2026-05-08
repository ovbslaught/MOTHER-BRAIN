#!/usr/bin/env python3
# OMEGA-CORE-01: Sketchfab-to-NOMADZ Template Converter
# Scans NOMADZ-0/Assets/Imports and moves them to the Character Template library.

import os
import shutil
import logging

IMPORT_DIR = "/storage/emulated/0/Wormhole/NOMADZ-0/Assets/Imports"
TEMPLATE_DIR = "/storage/emulated/0/Wormhole/NOMADZ-0/Assets/Characters/Templates"
LOG_FILE = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/archive/template_ingest.log"

os.makedirs(TEMPLATE_DIR, exist_ok=True)
logging.basicConfig(filename=LOG_FILE, level=logging.INFO, format='%(asctime)s - %(message)s')

def ingest_templates():
    """Moves downloaded Sketchfab models into the persistent rigging pipeline."""
    for item in os.listdir(IMPORT_DIR):
        if item.endswith((".zip", ".glb", ".gltf")):
            logging.info(f"cat> [INGESTER] New template detected: {item}")
            source = os.path.join(IMPORT_DIR, item)
            dest = os.path.join(TEMPLATE_DIR, item)
            shutil.move(source, dest)
            logging.info(f"cat> [INGESTER] {item} moved to Molecular Rigging Queue.")

if __name__ == "__main__":
    if os.path.exists(IMPORT_DIR):
        ingest_templates()
