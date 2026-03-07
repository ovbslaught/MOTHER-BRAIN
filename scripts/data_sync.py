#!/usr/bin/env python3
"""
Data Synchronization Script for MOTHER-BRAIN
Handles syncing between Google Drive, local storage, and GitHub
"""

import os
import sys
import json
import shutil
from pathlib import Path
from datetime import datetime
import hashlib

class DataSync:
    def __init__(self, config_path="config/sync_config.json"):
        self.config = self.load_config(config_path)
        self.sync_log = []
        
    def load_config(self, config_path):
        """Load sync configuration"""
        try:
            with open(config_path, 'r') as f:
                return json.load(f)
        except FileNotFoundError:
            return self.create_default_config(config_path)
    
    def create_default_config(self, config_path):
        """Create default sync configuration"""
        config = {
            "gdrive_path": "/mnt/gdrive",
            "local_path": "./data",
            "sync_patterns": ["*.json", "*.db", "*.txt"],
            "exclude_patterns": ["temp/*", "*.tmp"],
            "bidirectional": True
        }
        
        Path(config_path).parent.mkdir(parents=True, exist_ok=True)
        with open(config_path, 'w') as f:
            json.dump(config, f, indent=2)
        
        return config
    
    def get_file_hash(self, filepath):
        """Calculate MD5 hash of file"""
        hash_md5 = hashlib.md5()
        try:
            with open(filepath, "rb") as f:
                for chunk in iter(lambda: f.read(4096), b""):
                    hash_md5.update(chunk)
            return hash_md5.hexdigest()
        except Exception:
            return None
    
    def sync_file(self, source, destination):
        """Sync a single file"""
        try:
            source_path = Path(source)
            dest_path = Path(destination)
            
            if not source_path.exists():
                return False
            
            # Check if files are identical
            if dest_path.exists():
                source_hash = self.get_file_hash(source_path)
                dest_hash = self.get_file_hash(dest_path)
                
                if source_hash == dest_hash:
                    self.log_sync(f"Skipped (identical): {source_path.name}")
                    return True
            
            # Create destination directory if needed
            dest_path.parent.mkdir(parents=True, exist_ok=True)
            
            # Copy file
            shutil.copy2(source_path, dest_path)
            self.log_sync(f"Synced: {source_path.name} -> {dest_path}")
            return True
            
        except Exception as e:
            self.log_sync(f"Error syncing {source}: {e}")
            return False
    
    def sync_directory(self, source_dir, dest_dir, patterns=None):
        """Sync entire directory"""
        source_path = Path(source_dir)
        dest_path = Path(dest_dir)
        
        if not source_path.exists():
            print(f"Source directory not found: {source_dir}")
            return False
        
        patterns = patterns or self.config.get("sync_patterns", ["*"])
        synced_count = 0
        
        for pattern in patterns:
            for file_path in source_path.rglob(pattern):
                if file_path.is_file():
                    relative_path = file_path.relative_to(source_path)
                    dest_file = dest_path / relative_path
                    
                    if self.sync_file(file_path, dest_file):
                        synced_count += 1
        
        print(f"Synced {synced_count} files from {source_dir} to {dest_dir}")
        return True
    
    def bidirectional_sync(self, dir1, dir2):
        """Perform bidirectional sync between two directories"""
        print(f"Starting bidirectional sync: {dir1} <-> {dir2}")
        
        path1 = Path(dir1)
        path2 = Path(dir2)
        
        # Sync newer files from dir1 to dir2
        for file1 in path1.rglob("*"):
            if file1.is_file():
                rel_path = file1.relative_to(path1)
                file2 = path2 / rel_path
                
                if not file2.exists():
                    self.sync_file(file1, file2)
                else:
                    # Compare modification times
                    if file1.stat().st_mtime > file2.stat().st_mtime:
                        self.sync_file(file1, file2)
        
        # Sync newer files from dir2 to dir1
        for file2 in path2.rglob("*"):
            if file2.is_file():
                rel_path = file2.relative_to(path2)
                file1 = path1 / rel_path
                
                if not file1.exists():
                    self.sync_file(file2, file1)
                else:
                    if file2.stat().st_mtime > file1.stat().st_mtime:
                        self.sync_file(file2, file1)
        
        return True
    
    def log_sync(self, message):
        """Log sync operations"""
        timestamp = datetime.now().isoformat()
        log_entry = f"[{timestamp}] {message}"
        self.sync_log.append(log_entry)
        print(log_entry)
    
    def save_sync_log(self, log_file="logs/sync_log.txt"):
        """Save sync log to file"""
        Path(log_file).parent.mkdir(parents=True, exist_ok=True)
        with open(log_file, 'a') as f:
            f.write("\n".join(self.sync_log) + "\n")

if __name__ == "__main__":
    import argparse
    
    parser = argparse.ArgumentParser(description="Data Sync for MOTHER-BRAIN")
    parser.add_argument("--source", required=True, help="Source directory")
    parser.add_argument("--destination", required=True, help="Destination directory")
    parser.add_argument("--bidirectional", action="store_true", help="Enable bidirectional sync")
    parser.add_argument("--config", default="config/sync_config.json", help="Config file path")
    
    args = parser.parse_args()
    
    sync = DataSync(args.config)
    
    if args.bidirectional:
        sync.bidirectional_sync(args.source, args.destination)
    else:
        sync.sync_directory(args.source, args.destination)
    
    sync.save_sync_log()
