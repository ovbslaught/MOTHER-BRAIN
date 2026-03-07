# MOTHER-BRAIN Setup Guide

## Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/ovbslaught/MOTHER-BRAIN.git
cd MOTHER-BRAIN
git checkout Cosmic-key
```

### 2. Run Setup Script
```bash
python setup.py
```

This will:
- Create necessary directories
- Setup .env file from template
- Create placeholder files
- Make scripts executable (Unix/Linux)
- Install Python dependencies (optional)

### 3. Configure Environment

Edit `.env` file with your actual credentials:
```bash
PERPLEXITY_API_KEY=your_actual_key
OPENAI_API_KEY=your_actual_key
GITHUB_TOKEN=your_actual_token
```

### 4. Configure Sync Settings

Edit `config/sync_config.json` to match your paths:
- Set `gdrive_path` to your Google Drive mount point
- Set `termux_path` if using Termux
- Adjust sync patterns as needed

### 5. Test the System

```bash
# Check orchestration status
python scripts/orchestration_agent.py --task status

# Test data sync
python scripts/data_sync.py --source ./data --destination ./backups

# Test Godot automation
python scripts/godot_automation.py --project /path/to/godot/project --headless
```

## Platform-Specific Setup

### Windows
1. Run `setup.py` with Python
2. Manually set environment variables or use `.env` file
3. Use Windows paths in config files (e.g., `C:\\Users\\...`)

### Linux/Ubuntu
```bash
# Install system dependencies
sudo apt-get update
sudo apt-get install python3-pip blender godot3

# Run setup
python3 setup.py
```

### Termux (Android)
```bash
# Install required packages
pkg install python git openssh

# Clone and setup
git clone https://github.com/ovbslaught/MOTHER-BRAIN.git
cd MOTHER-BRAIN
python setup.py

# Setup SSH for remote sync
ssh-keygen -t rsa -b 4096
cat ~/.ssh/id_rsa.pub  # Copy to authorized systems
```

### Google Colab
See the Colab notebook for automated setup: `MOTHER-BRAIN_UNIFIED_SETUP.ipynb`

## Configuration Files

### orchestration_config.json
Controls AI model settings and automation scripts:
- Enable/disable specific AI models
- Set API endpoints
- Configure task scheduling
- Set logging preferences

### sync_config.json
Controls data synchronization:
- Platform paths (GDrive, GitHub, Termux)
- File patterns to sync/exclude
- Bidirectional sync settings
- Backup configuration

## GitHub Actions

The repository includes automated workflows:
- `autonomous-sync.yml` - Automated data synchronization

To enable:
1. Go to repository Settings → Secrets
2. Add required secrets (API keys, tokens)
3. Enable Actions in repository settings

## Troubleshooting

### Common Issues

**Import errors:**
```bash
pip install -r requirements.txt
```

**Permission denied on scripts:**
```bash
chmod +x scripts/*.py scripts/*.sh
```

**Google Drive not mounting:**
- Install google-drive-ocamlfuse (Linux)
- Or use rclone for cross-platform support

**API key errors:**
- Verify .env file exists and contains keys
- Check key format (no quotes or extra spaces)

## Next Steps

1. Read the main README.md for system overview
2. Check docs folder for additional documentation
3. Explore automation scripts in scripts/ folder
4. Test orchestration with sample tasks
5. Setup automated sync schedules

## Support

For issues or questions:
- Create an issue on GitHub
- Check existing documentation in docs/
- Review workflow files for automation examples
