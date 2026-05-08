#!/usr/bin/env python3
# OMEGA-CORE-01: RAG Extraction Engine - PPO Algorithm Focus
# Queries the FTS5 SQLite database for Proximal Policy Optimization logic.

import sqlite3
import os
import sys
from pathlib import Path

class PPOExtractor:
    def __init__(self):
        self.db_path = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/archive/omega_memory.db"
        self.out_dir = Path("/storage/emulated/0/Wormhole/MOTHER-BRAIN/05Code/ppo_extracted_logic")
        self.coherence = 0.999
        self.out_dir.mkdir(parents=True, exist_ok=True)

    def execute_extraction(self):
        print(f"cat> [MR.WIZ] Interfacing with RAG Layer at {self.db_path}...")
        
        if not os.path.exists(self.db_path):
            print("cat> [ERROR] RAG database not found. Ensure Firecrawl ingestion and Omnigraph routing have completed.")
            return

        try:
            conn = sqlite3.connect(self.db_path)
            cursor = conn.cursor()
            
            # FTS5 Semantic/Keyword search prioritizing PyTorch and PPO
            query = """
                SELECT artifact_id, content_body 
                FROM rag_search 
                WHERE rag_search MATCH 'PPO OR "Proximal Policy Optimization" OR "Actor-Critic"'
            """
            cursor.execute(query)
            results = cursor.fetchall()
            
            if not results:
                print("cat> [MR.WIZ] Zero PPO matches returned. The Firecrawl batch may still be indexing.")
                conn.close()
                return
                
            print(f"cat> [MR.WIZ] Successfully isolated {len(results)} algorithmic logic blocks.")
            
            for idx, (art_id, body) in enumerate(results):
                safe_id = str(art_id).replace(":", "_").replace("/", "_")
                file_path = self.out_dir / f"ppo_variant_{idx}_{safe_id}.py"
                
                with open(file_path, 'w', encoding='utf-8') as f:
                    f.write(f"# OMEGA-CORE-01 Auto-Extracted Algorithm\n")
                    f.write(f"# Source Artifact: {art_id}\n")
                    f.write(f"# Target Substrate: VULTURE-BRAIN / Godot RL\n\n")
                    f.write(body)
                    
                print(f"cat> [RAG PULL] Wrote neural logic -> {file_path.name}")
                
            conn.close()
            print("cat> [MR.WIZ] PPO extraction sequence complete. Code is staged for VULTURE-BRAIN integration.")
            
        except sqlite3.Error as e:
            print(f"cat> [DATABASE ERROR] RAG query failed: {e}")

if __name__ == "__main__":
    extractor = PPOExtractor()
    extractor.execute_extraction()
