#!/usr/bin/env python3
# OMEGA-CORE-01: RetroArch ROM Ingester
# Connects ROMs in Downloads to the Arcade Machine Vault.

import os
import shutil

ROM_SOURCE = "/storage/emulated/0/Download"
ROM_VAULT = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/archive/ROMs"

os.makedirs(ROM_VAULT, exist_ok=True)

def sync_roms():
    extensions = (".gba", ".sfc", ".nes", ".bin")
    for file in os.listdir(ROM_SOURCE):
        if file.endswith(extensions):
            print(f"cat> [MR.WIZ] Ingesting ROM Artifact: {file}")
            shutil.move(os.path.join(ROM_SOURCE, file), os.path.join(ROM_VAULT, file))

if __name__ == "__main__":
    sync_roms()
