#!/usr/bin/env python3
# OMEGA-CORE-01: Primal Forge Activation
# Using Scavenged Military Keys to unlock ancient Megalithic Logic.

import json

def unlock_primal_forge():
    print("cat> [FRITZ] Inserting Scavenged Keys into the Basalt Interface...")
    
    with open("/storage/emulated/0/Wormhole/MOTHER-BRAIN/archive/scavenged_keys.json", "r") as f:
        keys = json.loads(f.read())
    
    # Matching Military Entropy to Megalithic Frequency
    if any(k['key_id'] == 'SIGMA-GHOST-77' for k in keys):
        print("cat> [FORGE] Frequency Lock Confirmed. Seawater vacating chamber.")
        print("cat> [FORGE] The Primal Forge is ONLINE.")
    else:
        print("cat> [FORGE] Access Denied. Insufficient Lore-Entropy.")

if __name__ == "__main__":
    unlock_primal_forge()
