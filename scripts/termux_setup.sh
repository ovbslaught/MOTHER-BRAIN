#!/data/data/com.termux/files/usr/bin/bash
# MOTHER-BRAIN Termux SSH Setup Script
# Configures SSH, Git, and GitHub integration for autonomous operations

set -e

echo "🤖 MOTHER-BRAIN Termux Setup"
echo "================================"

# Update package manager
echo "📦 Updating packages..."
pkg update -y && pkg upgrade -y

# Install required packages
echo "📦 Installing core packages..."
pkg install -y openssh git python git-lfs rclone vim curl wget

# Setup SSH
echo "🔐 Configuring SSH..."
if [ ! -f "$HOME/.ssh/id_ed25519" ]; then
    ssh-keygen -t ed25519 -C "ovbslaught@gmail.com" -N "" -f "$HOME/.ssh/id_ed25519"
    echo "✅ SSH key generated"
else
    echo "✅ SSH key already exists"
fi

# Display SSH public key
echo ""
echo "📋 Your SSH public key (add to GitHub):"
echo "========================================"
cat "$HOME/.ssh/id_ed25519.pub"
echo "========================================"
echo ""

# Configure Git
echo "🔧 Configuring Git..."
git config --global user.name "ovbslaught"
git config --global user.email "ovbslaught@gmail.com"
git config --global init.defaultBranch main

# Setup GitHub CLI token
echo "🔑 Setting up GitHub authentication..."
if [ -n "$GH_PAT" ]; then
    git config --global credential.helper store
    echo "https://ovbslaught:${GH_PAT}@github.com" > ~/.git-credentials
    chmod 600 ~/.git-credentials
    echo "✅ GitHub token configured"
else
    echo "⚠️  GH_PAT not set. Export it manually:"
    echo "   export GH_PAT='your_github_token'"
fi

# Clone MOTHER-BRAIN repository
echo "📥 Cloning MOTHER-BRAIN repository..."
cd "$HOME"
if [ ! -d "MOTHER-BRAIN" ]; then
    git clone https://github.com/ovbslaught/MOTHER-BRAIN.git
    cd MOTHER-BRAIN
    git checkout Cosmic-key
    echo "✅ Repository cloned"
else
    echo "✅ Repository already exists"
    cd MOTHER-BRAIN
    git pull origin Cosmic-key
fi

# Configure rclone for Google Drive
echo "🌐 Setting up rclone..."
mkdir -p "$HOME/.config/rclone"

if [ -n "$GDRIVE_SERVICE_ACCOUNT_JSON" ]; then
    echo "$GDRIVE_SERVICE_ACCOUNT_JSON" > "$HOME/.config/rclone/gdrive-sa.json"
    
    cat > "$HOME/.config/rclone/rclone.conf" << 'EOF'
[gdrive]
type = drive
scope = drive
service_account_file = $HOME/.config/rclone/gdrive-sa.json
root_folder_id = 1VSH08EzxY0Knni5HKUaHePTliUAKxfWh
EOF
    
    chmod 600 "$HOME/.config/rclone/gdrive-sa.json"
    chmod 600 "$HOME/.config/rclone/rclone.conf"
    echo "✅ rclone configured for Google Drive"
else
    echo "⚠️  GDRIVE_SERVICE_ACCOUNT_JSON not set"
fi

# Install Python dependencies
echo "🐍 Installing Python packages..."
pip install --upgrade pip
pip install requests beautifulsoup4 google-auth google-auth-oauthlib google-auth-httplib2 google-api-python-client

# Setup SSH server
echo "🌐 Configuring SSH server..."
sshd

# Create sync script
echo "📝 Creating sync script..."
cat > "$HOME/MOTHER-BRAIN/sync.sh" << 'SYNCSCRIPT'
#!/data/data/com.termux/files/usr/bin/bash
# MOTHER-BRAIN Sync Script

cd "$HOME/MOTHER-BRAIN"

echo "🌀 Starting MOTHER-BRAIN sync..."

# Pull latest from GitHub
echo "📥 Pulling from GitHub..."
git pull origin Cosmic-key

# Sync from Google Drive
echo "☁️  Syncing from Drive..."
rclone sync gdrive:MOTHER-BRAIN ./MOTHER-BRAIN --checksum --verbose

# Commit and push changes
echo "📤 Pushing to GitHub..."
git add -A
if ! git diff --staged --quiet; then
    git commit -m "🤖 Termux auto-sync: $(date)"
    git push origin Cosmic-key
    echo "✅ Sync complete!"
else
    echo "✅ No changes to sync"
fi
SYNCSCRIPT

chmod +x "$HOME/MOTHER-BRAIN/sync.sh"

# Setup cron job for automatic syncing
echo "⏰ Setting up automatic sync (every 6 hours)..."
mkdir -p "$HOME/.termux/cron"
cat > "$HOME/.termux/cron/sync-motherbrain" << 'CRONSCRIPT'
0 */6 * * * bash $HOME/MOTHER-BRAIN/sync.sh >> $HOME/MOTHER-BRAIN/sync.log 2>&1
CRONSCRIPT

# Install termux-services if not already installed
pkg install -y termux-services
sv-enable crond

echo ""
echo "✅ MOTHER-BRAIN Termux setup complete!"
echo "================================"
echo ""
echo "📋 Next steps:"
echo "1. Add SSH public key to GitHub: https://github.com/settings/keys"
echo "2. Export secrets (if not already set):"
echo "   export GH_PAT='your_github_token'"
echo "   export GDRIVE_SERVICE_ACCOUNT_JSON='your_service_account_json'"
echo "3. Run manual sync: bash ~/MOTHER-BRAIN/sync.sh"
echo "4. Automatic sync runs every 6 hours via cron"
echo ""
echo "🔑 Your SSH public key:"
cat "$HOME/.ssh/id_ed25519.pub"
echo ""
