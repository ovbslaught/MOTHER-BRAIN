#!/bin/bash
# MOTHER-BRAIN Auto-Configuration Script
# Automatically finds keyz folder and sets up all MCP configs
# No more manual configuration stress!

set -e

echo "🧠 MOTHER-BRAIN Auto-Configuration Starting..."
echo "================================================"

# Color codes for pretty output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to find keyz folder in common locations
find_keyz_folder() {
    echo -e "${BLUE}🔍 Searching for keyz folder...${NC}"
    
    # Common locations to search
    SEARCH_PATHS=(
        "$HOME/Google Drive/keyz"
        "$HOME/GoogleDrive/keyz"
        "$HOME/Drive/keyz"
        "$HOME/gdrive/keyz"
        "/mnt/gdrive/keyz"
        "G:/keyz"
        "C:/Users/$USER/Google Drive/keyz"
        "$HOME/wormhole/keyz"
        "$HOME/Documents/keyz"
    )
    
    for path in "${SEARCH_PATHS[@]}"; do
        if [ -d "$path" ]; then
            echo -e "${GREEN}✅ Found keyz folder: $path${NC}"
            echo "$path"
            return 0
        fi
    done
    
    # If not found, try to find it recursively in home and common drives
    echo -e "${YELLOW}⚠️  Keyz folder not found in common locations, searching deeper...${NC}"
    
    KEYZ_PATH=$(find "$HOME" -type d -name "keyz" 2>/dev/null | head -1)
    if [ -n "$KEYZ_PATH" ]; then
        echo -e "${GREEN}✅ Found keyz folder: $KEYZ_PATH${NC}"
        echo "$KEYZ_PATH"
        return 0
    fi
    
    echo -e "${RED}❌ Could not find keyz folder automatically${NC}"
    read -p "Please enter the full path to your keyz folder: " MANUAL_PATH
    if [ -d "$MANUAL_PATH" ]; then
        echo "$MANUAL_PATH"
        return 0
    else
        echo -e "${RED}❌ Invalid path. Exiting.${NC}"
        exit 1
    fi
}

# Function to create .env file
create_env_file() {
    local keyz_path=$1
    echo -e "${BLUE}📝 Creating .env file...${NC}"
    
    ENV_FILE="$HOME/.mother-brain.env"
    
    cat > "$ENV_FILE" << EOF
# MOTHER-BRAIN Environment Configuration
# Auto-generated on $(date)
# Keyz folder location: $keyz_path

# Paths
KEYZ_FOLDER="$keyz_path"
GDRIVE_SERVICE_ACCOUNT="$keyz_path/gdrive-sa.json"
RCLONE_CONFIG="$HOME/.config/rclone/rclone.conf"

# Google Drive
GDRIVE_ROOT_FOLDER_ID="1VSH08EzxY0Knni5HKUaHePTliUAKxfWh"

# GitHub (Add your token to keyz/github_token.txt)
GITHUB_TOKEN=\$(cat "$keyz_path/github_token.txt" 2>/dev/null || echo "")

# AI API Keys (Add to keyz folder)
PERPLEXITY_API_KEY=\$(cat "$keyz_path/perplexity_api_key.txt" 2>/dev/null || echo "")
GEMINI_API_KEY=\$(cat "$keyz_path/gemini_api_key.txt" 2>/dev/null || echo "")
OPENAI_API_KEY=\$(cat "$keyz_path/openai_api_key.txt" 2>/dev/null || echo "")
ANTHROPIC_API_KEY=\$(cat "$keyz_path/anthropic_api_key.txt" 2>/dev/null || echo "")

# Notion
NOTION_API_KEY=\$(cat "$keyz_path/notion_api_key.txt" 2>/dev/null || echo "")

# Obsidian Vault Path
OBSIDIAN_VAULT_PATH="$HOME/Documents/Obsidian/Vault"

# Wormhole Paths
WORMHOLE_C="C:/wormhole"
WORMHOLE_D="D:/wormhole"
WORMHOLE_ANDROID="/sdcard/wormhole"
WORMHOLE_TEMPORAL="C:/workspace/wormhole/MOTHER_BRAIN/temporal_raw_data"
EOF

    echo -e "${GREEN}✅ .env file created: $ENV_FILE${NC}"
    echo -e "${YELLOW}📌 Add this to your shell profile: source $ENV_FILE${NC}"
}

# Function to create OpenCode MCP configuration
create_opencode_config() {
    local keyz_path=$1
    echo -e "${BLUE}⚙️  Creating OpenCode MCP configuration...${NC}"
    
    mkdir -p "$HOME/.config/opencode"
    
    cat > "$HOME/.config/opencode/opencode.json" << EOF
{
  "mcpServers": {
    "filesystem": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-filesystem"],
      "env": {
        "ALLOWED_DIRECTORIES": "C:\\\\wormhole;D:\\\\wormhole;C:\\\\workspace\\\\wormhole\\\\MOTHER_BRAIN\\\\temporal_raw_data;/sdcard/wormhole;$keyz_path;$HOME/Documents;$HOME/Downloads"
      }
    },
    "github": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-github"],
      "env": {
        "GITHUB_PERSONAL_ACCESS_TOKEN": "\${GITHUB_TOKEN}"
      }
    },
    "gdrive": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-gdrive"],
      "env": {
        "GOOGLE_APPLICATION_CREDENTIALS": "$keyz_path/gdrive-sa.json"
      }
    },
    "notion": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-notion"],
      "env": {
        "NOTION_API_KEY": "\${NOTION_API_KEY}"
      }
    },
    "brave-search": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-brave-search"],
      "env": {
        "BRAVE_API_KEY": "\${BRAVE_API_KEY}"
      }
    },
    "puppeteer": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-puppeteer"]
    }
  }
}
EOF

    echo -e "${GREEN}✅ OpenCode MCP config created: $HOME/.config/opencode/opencode.json${NC}"
}

# Function to create rclone configuration
create_rclone_config() {
    local keyz_path=$1
    echo -e "${BLUE}🔧 Creating rclone configuration...${NC}"
    
    mkdir -p "$HOME/.config/rclone"
    
    if [ -f "$keyz_path/gdrive-sa.json" ]; then
        cat > "$HOME/.config/rclone/rclone.conf" << EOF
[gdrive]
type = drive
scope = drive
service_account_file = $keyz_path/gdrive-sa.json
root_folder_id = 1VSH08EzxY0Knni5HKUaHePTliUAKxfWh

[wormhole]
type = alias
remote = gdrive:wormhole

[temporal]
type = alias
remote = gdrive:wormhole/MOTHER_BRAIN/temporal_raw_data
EOF
        echo -e "${GREEN}✅ rclone config created with service account${NC}"
    else
        echo -e "${YELLOW}⚠️  gdrive-sa.json not found in keyz folder${NC}"
        echo -e "${YELLOW}   Run 'rclone config' manually to set up Google Drive${NC}"
    fi
}

# Function to create API key template files
create_api_key_templates() {
    local keyz_path=$1
    echo -e "${BLUE}📋 Creating API key template files in keyz folder...${NC}"
    
    # Create template files for API keys if they don't exist
    touch "$keyz_path/github_token.txt"
    touch "$keyz_path/perplexity_api_key.txt"
    touch "$keyz_path/gemini_api_key.txt"
    touch "$keyz_path/openai_api_key.txt"
    touch "$keyz_path/anthropic_api_key.txt"
    touch "$keyz_path/notion_api_key.txt"
    touch "$keyz_path/brave_api_key.txt"
    
    echo -e "${GREEN}✅ API key template files created in: $keyz_path${NC}"
    echo -e "${YELLOW}📝 Paste your API keys into these files:${NC}"
    echo "   - github_token.txt"
    echo "   - perplexity_api_key.txt"
    echo "   - gemini_api_key.txt"
    echo "   - openai_api_key.txt"
    echo "   - anthropic_api_key.txt"
    echo "   - notion_api_key.txt"
}

# Function to create quick-start script
create_quickstart_script() {
    echo -e "${BLUE}🚀 Creating quick-start script...${NC}"
    
    cat > "$HOME/start-mother-brain.sh" << 'EOF'
#!/bin/bash
# Quick-start script for MOTHER-BRAIN
source "$HOME/.mother-brain.env"
echo "🧠 MOTHER-BRAIN Environment Loaded"
echo "=================================="
echo "Keyz folder: $KEYZ_FOLDER"
echo ""
echo "Available commands:"
echo "  opencode          - Start OpenCode SST"
echo "  rclone sync       - Sync with Google Drive"
echo "  git push/pull     - Git operations"
echo ""
EOF

    chmod +x "$HOME/start-mother-brain.sh"
    echo -e "${GREEN}✅ Quick-start script created: $HOME/start-mother-brain.sh${NC}"
}

# Main execution
main() {
    echo ""
    echo "🧠 MOTHER-BRAIN Auto-Configuration"
    echo "===================================="
    echo ""
    
    # Find keyz folder
    KEYZ_PATH=$(find_keyz_folder)
    
    if [ -z "$KEYZ_PATH" ]; then
        echo -e "${RED}❌ Setup failed: Could not locate keyz folder${NC}"
        exit 1
    fi
    
    echo ""
    echo -e "${GREEN}✅ Keyz folder located: $KEYZ_PATH${NC}"
    echo ""
    
    # Create configurations
    create_env_file "$KEYZ_PATH"
    echo ""
    
    create_opencode_config "$KEYZ_PATH"
    echo ""
    
    create_rclone_config "$KEYZ_PATH"
    echo ""
    
    create_api_key_templates "$KEYZ_PATH"
    echo ""
    
    create_quickstart_script
    echo ""
    
    # Summary
    echo "================================================"
    echo -e "${GREEN}🎉 Configuration Complete!${NC}"
    echo "================================================"
    echo ""
    echo "Next steps:"
    echo "1. Add API keys to files in: $KEYZ_PATH"
    echo "2. Source environment: source ~/.mother-brain.env"
    echo "3. Run quick-start: ~/start-mother-brain.sh"
    echo ""
    echo "OpenCode is configured at: ~/.config/opencode/opencode.json"
    echo "rclone is configured at: ~/.config/rclone/rclone.conf"
    echo ""
    echo -e "${BLUE}🌀 Wormhole automation is ready!${NC}"
    echo ""
}

# Run main function
main
