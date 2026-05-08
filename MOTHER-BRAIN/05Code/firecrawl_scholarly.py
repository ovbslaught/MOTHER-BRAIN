#!/usr/bin/env python3
import requests
import os

API_KEY = "fc-4e5a4f28d23e47e4986dba21f3bbbbf2"
VAULT_PATH = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/Obsidian_NOMADZ_Archive/04_TECHNICAL_LOGS"

def scholarly_scrape(target_url, filename):
    print(f"cat> [SCHOLAR-SYNC] Attempting Ingest: {target_url}")
    
    # Restricting ingest to reputable domains via logic check
    reputable_suffixes = (".edu", ".gov", ".org", "arxiv.org", "jstor.org", "nature.com")
    if not any(suffix in target_url for suffix in reputable_suffixes):
        print(f"cat> [BLOCK] URL '{target_url}' failed Reputable Source Protocol.")
        return

    headers = {"Authorization": f"Bearer {API_KEY}", "Content-Type": "application/json"}
    payload = {
        "url": target_url,
        "pageOptions": {"onlyMainContent": True},
        "extractorOptions": {"mode": "markdown"}
    }
    
    response = requests.post("https://api.firecrawl.dev/v0/scrape", headers=headers, json=payload)
    
    if response.status_code == 200:
        content = response.json().get('data', {}).get('markdown', '')
        with open(f"{VAULT_PATH}/{filename}.md", "w") as f:
            f.write(f"# SCHOLARLY INGEST: {filename.upper()}\n\nSource: {target_url}\n\n{content}")
        print(f"cat> [SUCCESS] Peer-reviewed logic secured to {filename}.md")
    else:
        print(f"cat> [ERROR] Siphon failed: {response.status_code}")

if __name__ == "__main__":
    # Staging area for your curated scholarly list
    # Targets for Geometric Certainty, AGI Ethics, and Sub-Atomic Physics
    targets = [
        ("https://arxiv.org/abs/2301.00001", "AGI_Neural_Foundations"),
        ("https://mit.edu/research/physics", "Geometric_Certainty_Substrate")
    ]
    for url, label in targets:
        scholarly_scrape(url, label)
