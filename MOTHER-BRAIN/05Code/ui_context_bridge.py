#!/usr/bin/env python3
# OMEGA-CORE-01: UI Context Bridge
# Exports the latest Phi and Reward metrics from SQLite to a JSON readable by Godot.

import sqlite3
import json
import os

DB_PATH = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/archive/omega_memory.db"
UI_JSON = "/storage/emulated/0/Wormhole/NOMADZ-0/Vulture/ui_telemetry.json"

def export_context():
    if not os.path.exists(DB_PATH): return
    
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    cursor.execute("SELECT phi, reward FROM training_log ORDER BY timestamp DESC LIMIT 1")
    row = cursor.fetchone()
    
    if row:
        data = {"phi": row[0], "reward": row[1]}
        with open(UI_JSON, 'w') as f:
            json.dump(data, f)
    conn.close()

if __name__ == "__main__":
    export_context()
