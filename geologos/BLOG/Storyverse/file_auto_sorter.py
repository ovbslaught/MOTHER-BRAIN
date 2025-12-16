import time
import sys
import os
import shutil
from typing import Dict

try:
    from watchdog.observers import Observer
    from watchdog.events import FileSystemEventHandler
except ImportError:
    print("❌ Watchdog not installed: pip install watchdog")
    sys.exit(1)

try:
    import yaml
except ImportError:
    print("❌ PyYAML not installed: pip install PyYAML")
    sys.exit(1)

class FileHandler(FileSystemEventHandler):
    def __init__(self, vault_path: str):
        self.vault_path = vault_path
        self.processing_queue = set()

    def on_created(self, event):
        if event.is_directory:
            return
        file_path = event.src_path
        if file_path in self.processing_queue or file_path.endswith('~'):
            return
        self.processing_queue.add(file_path)
        self.process_file(file_path)
        self.processing_queue.remove(file_path)

    def process_file(self, file_path: str):
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
                if content.startswith('---'):
                    end = content.find('---', 3)
                    if end > 0:
                        metadata_str = content[3:end]
                        metadata = yaml.safe_load(metadata_str)
                        file_type = metadata.get('type')
                        if file_type:
                            target_dir = self.determine_target_directory(file_type, metadata)
                            target_path = os.path.join(target_dir, os.path.basename(file_path))
                            if target_path != file_path:
                                os.makedirs(target_dir, exist_ok=True)
                                if os.path.exists(target_path):
                                    base, ext = os.path.splitext(target_path)
                                    target_path = f"{base}_{int(time.time())}{ext}"
                                shutil.move(file_path, target_path)
        except yaml.YAMLError as e:
            print(f"❌ YAML error in {file_path}: {e}")
            self.move_to_unsorted(file_path)
        except Exception as e:
            print(f"❌ Error processing {file_path}: {e}")
            self.move_to_unsorted(file_path)
    
    def move_to_unsorted(self, file_path: str):
        """Move problematic files to unsorted folder"""
        try:
            unsorted = os.path.join(self.vault_path, '_unsorted')
            os.makedirs(unsorted, exist_ok=True)
            target_path = os.path.join(unsorted, os.path.basename(file_path))
            if os.path.exists(target_path):
                base, ext = os.path.splitext(target_path)
                target_path = f"{base}_{int(time.time())}{ext}"
            shutil.move(file_path, target_path)
        except Exception as e:
            print(f"❌ Failed to move to unsorted: {e}")

    def determine_target_directory(self, file_type: str, metadata: Dict) -> str:
        base = os.path.join(self.vault_path, file_type.capitalize() + "s")
        if file_type == 'character':
            archetype = metadata.get('archetype', 'supporting')
            return os.path.join(base, archetype + "s" if archetype in ['protagonist', 'antagonist'] else archetype)
        elif file_type == 'location':
            loc_type = metadata.get('location_type', 'landmarks')
            return os.path.join(base, loc_type)
        elif file_type == 'quest':
            q_type = metadata.get('quest_type', 'quest_chains')
            return os.path.join(base, q_type.replace('_', '_'))
        # Add for item, faction
        return base

if __name__ == "__main__":
    # Load configuration with error handling
    try:
        config_path = 'config.yaml'
        if not os.path.exists(config_path):
            print(f"❌ Config file not found: {config_path}")
            print("Creating default config...")
            default_config = {
                'sorting': {
                    'conflict_resolution': 'rename'
                }
            }
            with open(config_path, 'w') as f:
                yaml.dump(default_config, f, default_flow_style=False)
            config = default_config
        else:
            config = yaml.safe_load(open(config_path, 'r', encoding='utf-8'))
    except Exception as e:
        print(f"❌ Error loading config: {e}")
        config = {'sorting': {'conflict_resolution': 'rename'}}
    
    # Get vault path
    vault_path = os.getenv('OBSIDIAN_VAULT_PATH')
    if not vault_path:
        print("❌ OBSIDIAN_VAULT_PATH environment variable not set")
        print("Usage: export OBSIDIAN_VAULT_PATH=/path/to/your/vault")
        sys.exit(1)
    
    if not os.path.exists(vault_path):
        print(f"❌ Vault path does not exist: {vault_path}")
        sys.exit(1)
    
    print(f"📁 Starting file sorter for vault: {vault_path}")
    
    try:
        event_handler = FileHandler(vault_path)
        observer = Observer()
        observer.schedule(event_handler, vault_path, recursive=True)
        observer.start()
        print("✅ File sorter started. Press Ctrl+C to stop.")
        
        try:
            while True:
                time.sleep(1)
        except KeyboardInterrupt:
            print("\n🛑 Stopping file sorter...")
            observer.stop()
        observer.join()
        print("✅ File sorter stopped")
    except Exception as e:
        print(f"❌ Error starting file sorter: {e}")
        sys.exit(1)