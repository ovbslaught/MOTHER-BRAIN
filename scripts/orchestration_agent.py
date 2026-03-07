#!/usr/bin/env python3
"""
Orchestration Agent for MOTHER-BRAIN
Coordinates multiple AI models and automation tasks
"""

import os
import sys
import json
import time
from datetime import datetime
from pathlib import Path
import subprocess
import requests

class OrchestrationAgent:
    def __init__(self, config_path="config/orchestration_config.json"):
        self.config = self.load_config(config_path)
        self.active_tasks = {}
        self.task_queue = []
        self.log_file = "logs/orchestration.log"
        
    def load_config(self, config_path):
        """Load orchestration configuration"""
        try:
            with open(config_path, 'r') as f:
                return json.load(f)
        except FileNotFoundError:
            return self.create_default_config(config_path)
    
    def create_default_config(self, config_path):
        """Create default orchestration configuration"""
        config = {
            "models": {
                "perplexity": {
                    "api_key_env": "PERPLEXITY_API_KEY",
                    "enabled": True
                },
                "openai": {
                    "api_key_env": "OPENAI_API_KEY",
                    "enabled": False
                }
            },
            "automation": {
                "blender_script": "scripts/blender_automation.py",
                "godot_script": "scripts/godot_automation.py",
                "sync_script": "scripts/data_sync.py"
            },
            "max_concurrent_tasks": 3,
            "retry_attempts": 3
        }
        
        Path(config_path).parent.mkdir(parents=True, exist_ok=True)
        with open(config_path, 'w') as f:
            json.dump(config, f, indent=2)
        
        return config
    
    def log(self, message, level="INFO"):
        """Log orchestration events"""
        timestamp = datetime.now().isoformat()
        log_entry = f"[{timestamp}] [{level}] {message}"
        print(log_entry)
        
        Path(self.log_file).parent.mkdir(parents=True, exist_ok=True)
        with open(self.log_file, 'a') as f:
            f.write(log_entry + "\n")
    
    def execute_script(self, script_path, args=None):
        """Execute a Python script with arguments"""
        cmd = [sys.executable, script_path]
        if args:
            cmd.extend(args)
        
        try:
            self.log(f"Executing: {' '.join(cmd)}")
            result = subprocess.run(cmd, capture_output=True, text=True, timeout=300)
            
            if result.returncode == 0:
                self.log(f"Script completed: {script_path}")
                return {"success": True, "output": result.stdout}
            else:
                self.log(f"Script failed: {result.stderr}", "ERROR")
                return {"success": False, "error": result.stderr}
        except subprocess.TimeoutExpired:
            self.log(f"Script timeout: {script_path}", "ERROR")
            return {"success": False, "error": "Timeout"}
        except Exception as e:
            self.log(f"Script error: {e}", "ERROR")
            return {"success": False, "error": str(e)}
    
    def run_blender_automation(self):
        """Run Blender automation script"""
        script = self.config["automation"]["blender_script"]
        return self.execute_script(script)
    
    def run_godot_automation(self, action="headless"):
        """Run Godot automation script"""
        script = self.config["automation"]["godot_script"]
        args = [f"--{action}"]
        return self.execute_script(script, args)
    
    def run_data_sync(self, source, destination, bidirectional=False):
        """Run data synchronization"""
        script = self.config["automation"]["sync_script"]
        args = ["--source", source, "--destination", destination]
        if bidirectional:
            args.append("--bidirectional")
        return self.execute_script(script, args)
    
    def queue_task(self, task_type, task_params):
        """Add task to queue"""
        task = {
            "id": len(self.task_queue) + 1,
            "type": task_type,
            "params": task_params,
            "status": "queued",
            "created_at": datetime.now().isoformat()
        }
        self.task_queue.append(task)
        self.log(f"Task queued: {task_type} (ID: {task['id']})")
        return task["id"]
    
    def process_task_queue(self):
        """Process all queued tasks"""
        self.log(f"Processing {len(self.task_queue)} tasks")
        
        while self.task_queue:
            # Limit concurrent tasks
            if len(self.active_tasks) >= self.config["max_concurrent_tasks"]:
                time.sleep(1)
                continue
            
            task = self.task_queue.pop(0)
            task["status"] = "running"
            self.active_tasks[task["id"]] = task
            
            # Execute task based on type
            if task["type"] == "blender":
                result = self.run_blender_automation()
            elif task["type"] == "godot":
                result = self.run_godot_automation(task["params"].get("action"))
            elif task["type"] == "sync":
                result = self.run_data_sync(**task["params"])
            else:
                result = {"success": False, "error": "Unknown task type"}
            
            task["status"] = "completed" if result["success"] else "failed"
            task["result"] = result
            del self.active_tasks[task["id"]]
            
            self.log(f"Task {task['id']} {task['status']}")
    
    def get_status(self):
        """Get orchestration status"""
        return {
            "active_tasks": len(self.active_tasks),
            "queued_tasks": len(self.task_queue),
            "tasks": list(self.active_tasks.values())
        }

if __name__ == "__main__":
    import argparse
    
    parser = argparse.ArgumentParser(description="Orchestration Agent for MOTHER-BRAIN")
    parser.add_argument("--task", choices=["blender", "godot", "sync", "status"], help="Task type")
    parser.add_argument("--params", help="Task parameters (JSON string)")
    parser.add_argument("--config", default="config/orchestration_config.json", help="Config file")
    
    args = parser.parse_args()
    
    agent = OrchestrationAgent(args.config)
    
    if args.task == "status":
        print(json.dumps(agent.get_status(), indent=2))
    elif args.task:
        params = json.loads(args.params) if args.params else {}
        task_id = agent.queue_task(args.task, params)
        agent.process_task_queue()
        print(f"Task {task_id} completed")
