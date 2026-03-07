#!/usr/bin/env python3
"""
MOTHER-BRAIN Setup Script
Initializes the system, creates directories, and configures environment
"""

import os
import sys
import json
import shutil
from pathlib import Path

def create_directory_structure():
    """Create necessary directories"""
    directories = [
        "data",
        "logs",
        "temp",
        "output",
        "backups",
        "credentials",
        "scripts",
        "config",
        "docs",
        ".github/workflows"
    ]
    
    print("Creating directory structure...")
    for directory in directories:
        path = Path(directory)
        path.mkdir(parents=True, exist_ok=True)
        print(f"  ✓ Created: {directory}")

def setup_environment():
    """Setup environment file"""
    env_example = Path(".env.example")
    env_file = Path(".env")
    
    if not env_file.exists() and env_example.exists():
        print("\nCreating .env file from template...")
        shutil.copy(env_example, env_file)
        print("  ✓ Created .env file")
        print("  ⚠  Please edit .env and add your API keys!")
    else:
        print("\n.env file already exists, skipping...")

def install_dependencies():
    """Install Python dependencies"""
    requirements_file = Path("requirements.txt")
    
    if requirements_file.exists():
        print("\nInstalling Python dependencies...")
        try:
            import subprocess
            subprocess.run([sys.executable, "-m", "pip", "install", "-r", "requirements.txt"], 
                         check=True)
            print("  ✓ Dependencies installed")
        except subprocess.CalledProcessError:
            print("  ✗ Failed to install dependencies")
            print("  Run manually: pip install -r requirements.txt")
    else:
        print("\nNo requirements.txt found, skipping dependency installation...")

def create_placeholder_files():
    """Create placeholder files in directories"""
    placeholders = [
        ("data/.gitkeep", ""),
        ("logs/.gitkeep", ""),
        ("temp/.gitkeep", ""),
        ("output/.gitkeep", ""),
        ("backups/.gitkeep", ""),
    ]
    
    print("\nCreating placeholder files...")
    for filepath, content in placeholders:
        path = Path(filepath)
        if not path.exists():
            path.write_text(content)
            print(f"  ✓ Created: {filepath}")

def make_scripts_executable():
    """Make scripts executable (Unix-like systems)"""
    if sys.platform != "win32":
        print("\nMaking scripts executable...")
        scripts_dir = Path("scripts")
        if scripts_dir.exists():
            for script in scripts_dir.glob("*.py"):
                os.chmod(script, 0o755)
                print(f"  ✓ Made executable: {script.name}")
            for script in scripts_dir.glob("*.sh"):
                os.chmod(script, 0o755)
                print(f"  ✓ Made executable: {script.name}")

def display_next_steps():
    """Display next steps for the user"""
    print("\n" + "="*50)
    print("MOTHER-BRAIN Setup Complete!")
    print("="*50)
    print("\nNext steps:")
    print("1. Edit .env file with your API keys")
    print("2. Configure config/orchestration_config.json")
    print("3. Configure config/sync_config.json")
    print("4. Run: python scripts/orchestration_agent.py --task status")
    print("5. Test sync: python scripts/data_sync.py --source ./data --destination /backup")
    print("\nFor more information, see docs/SETUP.md")
    print("="*50 + "\n")

if __name__ == "__main__":
    print("\n🧠 MOTHER-BRAIN Setup Initializing...\n")
    
    create_directory_structure()
    setup_environment()
    create_placeholder_files()
    make_scripts_executable()
    
    # Ask about installing dependencies
    response = input("\nInstall Python dependencies now? (y/n): ").lower()
    if response == 'y':
        install_dependencies()
    else:
        print("Skipping dependency installation.")
        print("You can install later with: pip install -r requirements.txt")
    
    display_next_steps()
