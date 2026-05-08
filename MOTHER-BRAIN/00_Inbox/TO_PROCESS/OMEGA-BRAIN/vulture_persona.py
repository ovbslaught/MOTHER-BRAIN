import json
from pathlib import Path

# --- CONFIG ---
LOGS = Path("/sdcard/Wormhole/MOTHER-BRAIN/logs/swarm_training.jsonl")
PERSONA_DIR = Path("/sdcard/Wormhole/ARCHIVE/Obsidian/NOMADZ_VAULT/Story_Bible/Characters")
NOTION_STAGE = Path("/sdcard/Wormhole/MOTHER-BRAIN/00_inbox/TO_PROCESS/Notion_Sync")

def get_personality_trait(reward):
    if reward > 0.8: return "Precision-Focused / Elite"
    if reward > 0.6: return "Adaptive / Curious"
    return "Erratic / Fragmented"

def generate_character_sheet(data):
    name = f"Drone-{data['session'][-4:]}"
    reward = data['reward']
    trait = get_personality_trait(reward)
    
    sheet = f"""---
name: {name}
faction: Fracture Team
resonance_score: {reward}
trait: {trait}
last_seen: {data['timestamp']}
---
# Character Profile: {name}
> "The desert doesn't just change the code; it changes the intent."

## Behavioral Analysis
Based on RL Training Step {data['step']}, this unit displays **{trait}** tendencies. 
Its ability to parse the crystalline lattice is currently at **{reward * 100}%** efficiency.

## Logged Memory Fragments
- [X] Sector 7 Navigation Confirmed
- [ ] Ouroboros-7 Handshake Pending
"""
    return name, sheet

def run_persona_gen():
    PERSONA_DIR.mkdir(parents=True, exist_ok=True)
    NOTION_STAGE.mkdir(parents=True, exist_ok=True)
    if not LOGS.exists(): return

    with open(LOGS, "r") as f:
        for line in f:
            data = json.loads(line)
            name, content = generate_character_sheet(data)
            
            # Save to Obsidian
            with open(PERSONA_DIR / f"{name}.md", "w") as out:
                out.write(content)
            
            # Stage for Notion
            with open(NOTION_STAGE / f"{name}_notion.md", "w") as out:
                out.write(content)

if __name__ == "__main__":
    run_persona_gen()
