#!/bin/bash
# MOTHER-BRAIN Deploy Script
# Run on GCP VM (omega-brain-vm) or any Docker host
# Usage: bash deploy.sh

set -e

echo "=== MOTHER-BRAIN DEPLOY ==="
echo "Starting at: $(date)"

# 1. Install Docker if not present
if ! command -v docker &> /dev/null; then
    echo "Installing Docker..."
    curl -fsSL https://get.docker.com | bash
    sudo usermod -aG docker $USER
    echo "Docker installed. You may need to re-login for group changes."
fi

# 2. Install Docker Compose if not present
if ! command -v docker-compose &> /dev/null; then
    echo "Installing Docker Compose..."
    sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" \
        -o /usr/local/bin/docker-compose
    sudo chmod +x /usr/local/bin/docker-compose
fi

# 3. Clone or update MOTHER-BRAIN repo
REPO_DIR="/opt/mother-brain"
if [ -d "$REPO_DIR" ]; then
    echo "Updating MOTHER-BRAIN repo..."
    cd $REPO_DIR
    git fetch origin
    git checkout Cosmic-key
    git pull origin Cosmic-key
else
    echo "Cloning MOTHER-BRAIN repo..."
    sudo git clone https://github.com/ovbslaught/MOTHER-BRAIN.git $REPO_DIR
    cd $REPO_DIR
    git checkout Cosmic-key
fi

cd $REPO_DIR

# 4. Create data directories
echo "Creating data directories..."
mkdir -p /data/brain-hole
mkdir -p /data/admin-queue
mkdir -p /data/approved
mkdir -p /data/published
mkdir -p /data/drive
mkdir -p /data/chroma

# 5. Check for .env file
if [ ! -f ".env" ]; then
    echo "WARNING: No .env file found!"
    echo "Copy .env.example to .env and fill in your API keys:"
    echo "  cp .env.example .env && nano .env"
    echo ""
    echo "Required keys:"
    echo "  OPENAI_API_KEY"
    echo "  TELEGRAM_BOT_TOKEN"
    echo "  TELEGRAM_ADMIN_ID"
    echo "  LINEAR_API_KEY"
    echo ""
    read -p "Press Enter to continue with empty .env (not recommended)..."
    cp .env.example .env 2>/dev/null || touch .env
fi

# 6. Build and start containers
echo "Building Docker images..."
docker-compose build --no-cache

echo "Starting MOTHER-BRAIN stack..."
docker-compose up -d

# 7. Show status
echo ""
echo "=== MOTHER-BRAIN STACK STATUS ==="
docker-compose ps

echo ""
echo "=== CONTAINER LOGS (last 20 lines) ==="
docker-compose logs --tail=20

echo ""
echo "MOTHER-BRAIN is ONLINE."
echo "Monitor with: docker-compose logs -f"
echo "Admin Telegram bot ready - message your bot to control the system."
echo "Deploy complete at: $(date)"
