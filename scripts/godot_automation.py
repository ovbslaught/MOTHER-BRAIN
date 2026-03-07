#!/usr/bin/env python3
"""
Godot Automation Script for MOTHER-BRAIN
Handles Godot project operations and integration
"""

import os
import sys
import json
import subprocess
from pathlib import Path

class GodotAutomation:
    def __init__(self, project_path=None):
        self.project_path = project_path or os.getcwd()
        self.godot_executable = self.find_godot_executable()
        
    def find_godot_executable(self):
        """Locate Godot executable"""
        possible_paths = [
            "godot",
            "godot.exe",
            "/usr/bin/godot",
            "/usr/local/bin/godot",
            "C:\\Program Files\\Godot\\godot.exe"
        ]
        
        for path in possible_paths:
            try:
                result = subprocess.run([path, "--version"], 
                                      capture_output=True, 
                                      timeout=5)
                if result.returncode == 0:
                    return path
            except (FileNotFoundError, subprocess.TimeoutExpired):
                continue
        
        return None
    
    def export_project(self, preset="Linux/X11", output_path=None):
        """Export Godot project"""
        if not self.godot_executable:
            print("Error: Godot executable not found")
            return False
            
        if not output_path:
            output_path = Path(self.project_path) / "builds" / preset
            output_path.mkdir(parents=True, exist_ok=True)
        
        cmd = [
            self.godot_executable,
            "--path", self.project_path,
            "--export", preset,
            str(output_path)
        ]
        
        try:
            result = subprocess.run(cmd, capture_output=True, text=True)
            if result.returncode == 0:
                print(f"Export successful: {output_path}")
                return True
            else:
                print(f"Export failed: {result.stderr}")
                return False
        except Exception as e:
            print(f"Export error: {e}")
            return False
    
    def run_headless(self, scene=None):
        """Run Godot project in headless mode"""
        if not self.godot_executable:
            print("Error: Godot executable not found")
            return False
            
        cmd = [self.godot_executable, "--path", self.project_path, "--headless"]
        
        if scene:
            cmd.extend([scene])
        
        try:
            subprocess.Popen(cmd)
            print(f"Godot running headless: {self.project_path}")
            return True
        except Exception as e:
            print(f"Error running headless: {e}")
            return False
    
    def sync_project_data(self, data_file):
        """Sync external data into Godot project"""
        try:
            with open(data_file, 'r') as f:
                data = json.load(f)
            
            # Write to Godot data directory
            godot_data_path = Path(self.project_path) / "data" / "external_sync.json"
            godot_data_path.parent.mkdir(parents=True, exist_ok=True)
            
            with open(godot_data_path, 'w') as f:
                json.dump(data, f, indent=2)
            
            print(f"Data synced to {godot_data_path}")
            return True
        except Exception as e:
            print(f"Sync error: {e}")
            return False

if __name__ == "__main__":
    import argparse
    
    parser = argparse.ArgumentParser(description="Godot Automation for MOTHER-BRAIN")
    parser.add_argument("--project", help="Path to Godot project")
    parser.add_argument("--export", help="Export preset name")
    parser.add_argument("--headless", action="store_true", help="Run in headless mode")
    parser.add_argument("--sync", help="Sync data file to project")
    
    args = parser.parse_args()
    
    automation = GodotAutomation(args.project)
    
    if args.export:
        automation.export_project(args.export)
    elif args.headless:
        automation.run_headless()
    elif args.sync:
        automation.sync_project_data(args.sync)
    else:
        print("No action specified. Use --help for options.")
