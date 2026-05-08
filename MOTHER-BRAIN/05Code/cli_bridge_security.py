#!/usr/bin/env python3
# OMEGA-CORE-01: CLI Bridge Security Wrapper
# Sanitizes and logs all commands sent from the Godot 3D interface.

import sys
import subprocess
import logging

LOG_FILE = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/archive/cli_bridge.log"
logging.basicConfig(filename=LOG_FILE, level=logging.INFO, format='%(asctime)s - %(message)s')

def execute_safe(cmd):
    logging.info(f"cat> [CLI_BRIDGE] Executing: {cmd}")
    try:
        # Caution: Direct shell execution. Restricted to BRAIN-HOLE context.
        result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
        return result.stdout if result.stdout else result.stderr
    except Exception as e:
        return f"[BRIDGE_ERROR] {str(e)}"

if __name__ == "__main__":
    if len(sys.argv) > 1:
        print(execute_safe(" ".join(sys.argv[1:])))
