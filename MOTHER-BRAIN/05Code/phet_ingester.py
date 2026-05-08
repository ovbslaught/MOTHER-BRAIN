#!/usr/bin/env python3
# OMEGA-CORE-01: PhET HTML5 Ingester
# Downloads and packages PhET sims for offline Arcade Lab use.

import os
import requests

SIM_DIR = "/storage/emulated/0/Wormhole/NOMADZ-0/Assets/Labs/PhET"
os.makedirs(SIM_DIR, exist_ok=True)

# Common PhET HTML5 simulation URLs
PHET_SIMS = {
    "gravity": "https://phet.colorado.edu/sims/html/gravity-and-orbits/latest/gravity-and-orbits_all.html",
    "evolution": "https://phet.colorado.edu/sims/html/natural-selection/latest/natural-selection_all.html"
}

def download_sims():
    for name, url in PHET_SIMS.items():
        print(f"cat> [MR.WIZ] Acquiring PhET Lab artifact: {name}")
        r = requests.get(url)
        with open(os.path.join(SIM_DIR, f"{name}.html"), 'wb') as f:
            f.write(r.content)

if __name__ == "__main__":
    download_sims()
