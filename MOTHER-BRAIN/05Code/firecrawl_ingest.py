#!/usr/bin/env python3
import requests
import os
import json

API_KEY = "fc-4e5a4f28d23e47e4986dba21f3bbbbf2"
VAULT_PATH = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/Obsidian_NOMADZ_Archive/05_MANIFESTOS"

def crawl_and_write(url, filename):
    print(f"cat> [FIRECRAWL] Crawling: {url}")
    headers = {
        "Authorization": f"Bearer {API_KEY}",
        "Content-Type": "application/json"
    }
    data = {
        "url": url,
        "pageOptions": {"onlyMainContent": True},
        "extractorOptions": {"mode": "markdown"}
    }
    
    # Firecrawl Scrape API Endpoint
    response = requests.post("https://api.firecrawl.dev/v0/scrape", headers=headers, json=data)
    
    if response.status_code == 200:
        md_content = response.json().get('data', {}).get('markdown', '')
        file_path = f"{VAULT_PATH}/{filename}.md"
        with open(file_path, "w") as f:
            f.write(f"# Siphoned Content: {filename}\n\nSource: {url}\n\n---\n\n{md_content}")
        print(f"cat> [LIBRARIAN] Blog material written to {file_path}")
    else:
        print(f"cat> [ERROR] Firecrawl failed: {response.text}")

if __name__ == "__main__":
    # Example Target: Universal Logic / Geometric Certainty sources
    # Update these with your specific P2P or reputable resource URLs
    targets = {
        "Geometric_Foundations": "https://example.com/geometry-logic", 
        "Decolonial_AGI": "https://example.com/decolonial-tech"
    }
    for name, url in targets.items():
        crawl_and_write(url, name)
