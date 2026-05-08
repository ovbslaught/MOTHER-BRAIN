#!/usr/bin/env python3
import json
from http.server import BaseHTTPRequestHandler, HTTPServer

# Load the siphoned character data
with open("/storage/emulated/0/Wormhole/NOMADZ-0/Vulture/Extended_Roster.json", "r") as f:
    CHAR_DATA = json.load(f)

class VultureBridge(BaseHTTPRequestHandler):
    def do_GET(self):
        self.send_response(200)
        self.send_header("Content-type", "application/json")
        self.end_headers()
        
        # Extract character name from path (e.g., /JAX)
        char_name = self.path.strip("/")
        response = CHAR_DATA.get(char_name, {"error": "Character not found"})
        
        self.wfile.write(json.dumps(response).encode())

def run_bridge():
    server = HTTPServer(('localhost', 8080), VultureBridge)
    print("cat> [BRIDGE] Vulture-Logic Server running on port 8080...")
    server.serve_forever()

if __name__ == "__main__":
    run_bridge()
