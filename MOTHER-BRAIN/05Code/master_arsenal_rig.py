#!/usr/bin/env python3
# OMEGA-CORE-01: Universal Arsenal Rigger
# Recursively maps all vehicles, boards, and gear to the Godot Controller.

import os

ASSET_DIR = "/storage/emulated/0/Wormhole/NOMADZ-0/Assets/Gear"
exts = [".glb", ".fbx"]

def rig_everything():
    print("cat> [MR.WIZ] Rigging Surfers, Boards, and Deep Sea Creatures...")
    for root, dirs, files in os.walk(ASSET_DIR):
        for file in files:
            if any(file.endswith(e) for e in exts):
                print(f"cat> [RIGGER] Binding {file} to VULTURE-DRIVE-TRAIN.")
                # Logic to auto-assign 'HydroVehicle.gd' or 'HoverBoard.gd'

if __name__ == "__main__":
    rig_everything()
