import json
import gspread
from google.oauth2.service_account import Credentials
from pathlib import Path

# --- CONFIG ---
LOGS = Path("/sdcard/Wormhole/MOTHER-BRAIN/logs/swarm_training.jsonl")
SHEET_NAME = "Omega Space"
SCOPES = ["https://www.googleapis.com/auth/spreadsheets", "https://www.googleapis.com/auth/drive"]
CREDS = "/sdcard/Wormhole/OMEGA-BRAIN/service_account.json"

def sync_to_sheets():
    if not LOGS.exists(): 
        print("📭 No logs found for Sheet sync.")
        return
    
    try:
        # Auth Handshake
        credentials = Credentials.from_service_account_file(CREDS, scopes=SCOPES)
        gc = gspread.authorize(credentials)
        sh = gc.open(SHEET_NAME)
        worksheet = sh.get_worksheet(0) # Targeting First Tab
        
        # Extract and Append
        with open(LOGS, "r") as f:
            for line in f:
                data = json.loads(line)
                row = [
                    data.get('timestamp'), 
                    data.get('session'), 
                    data.get('step'), 
                    data.get('reward'), 
                    data.get('coherence')
                ]
                worksheet.append_row(row)
        
        print(f"📊 [Vulture-Sheet] Telemetry synced to '{SHEET_NAME}'.")
    except Exception as e:
        print(f"❌ Sheet Sync Failed: {e}")

if __name__ == "__main__":
    sync_to_sheets()
