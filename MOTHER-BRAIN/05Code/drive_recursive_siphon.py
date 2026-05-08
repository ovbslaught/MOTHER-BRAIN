#!/usr/bin/env python3
# OMEGA-CORE-01: Sept-to-March Google Drive Siphon
# Recursively crawls and downloads all historical development artifacts.

import os
import subprocess
import logging
from pathlib import Path

# Paths
DRIVE_REMOTE = "gdrive:NOMADZ-ARCHIVE" # Assumes rclone remote 'gdrive' is configured
LOCAL_INBOX = Path("/storage/emulated/0/Wormhole/MOTHER-BRAIN/00_Inbox/TO_PROCESS")
LOG_FILE = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/archive/siphon_crawl.log"

logging.basicConfig(filename=LOG_FILE, level=logging.INFO, format='%(asctime)s - %(message)s')

def siphon_historical_data():
    """Triggers rclone to sync all data from the Sept-March window."""
    print("cat> [SIPHON] Crawling Google Drive for historical molecular data...")
    
    try:
        # Pulling everything from the cloud to the local processing inbox
        # --include patterns for scripts, models, and logs
        subprocess.run([
            "rclone", "copy", DRIVE_REMOTE, str(LOCAL_INBOX),
            "--include", "*.{py,gd,tscn,json,md,pdf,zip,glb}",
            "--verbose"
        ], check=True)
        
        logging.info("cat> [SIPHON] Deep crawl complete. Data staged in TO_PROCESS.")
    except Exception as e:
        logging.error(f"cat> [SIPHON ERROR] Crawl failed: {e}")

if __name__ == "__main__":
    siphon_historical_data()
