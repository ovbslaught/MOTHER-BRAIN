import asyncio
import os
import json
import yaml
import random
import sys
from datetime import datetime
from typing import Dict

# Optional imports with error handling
try:
    from ollama import AsyncClient
except ImportError:
    print("⚠️ Ollama not installed: pip install ollama")
    AsyncClient = None

try:
    import google.generativeai as genai
except ImportError:
    print("⚠️ Google Generative AI not installed: pip install google-generativeai")
    genai = None

try:
    from mcp.client import Client  # MCP Client for calling servers
except ImportError:
    print("⚠️ MCP Client not available")
    Client = None

class UniverseEngine:
    def __init__(self):
        # Load configuration with error handling
        try:
            config_path = 'config.yaml'
            if not os.path.exists(config_path):
                print(f"⚠️ Config file not found: {config_path}, using defaults")
                self.config = self.get_default_config()
            else:
                self.config = yaml.safe_load(open(config_path, 'r', encoding='utf-8'))
        except Exception as e:
            print(f"⚠️ Error loading config: {e}, using defaults")
            self.config = self.get_default_config()
        
        self.vault_path = os.getenv('OBSIDIAN_VAULT_PATH')
        self.godot_path = os.getenv('GODOT_PROJECT_PATH')
        
        # Initialize clients with error handling
        self.ollama_client = None
        if AsyncClient:
            try:
                ollama_host = os.getenv('OLLAMA_HOST', 'http://localhost:11434')
                self.ollama_client = AsyncClient(host=ollama_host)
            except Exception as e:
                print(f"⚠️ Failed to initialize Ollama client: {e}")
        
        self.gemini_api_key = os.getenv('GEMINI_API_KEY')
        if self.gemini_api_key and genai:
            try:
                genai.configure(api_key=self.gemini_api_key)
            except Exception as e:
                print(f"⚠️ Failed to configure Gemini: {e}")
        
        self.session_history = []
        
        # Initialize MCP clients if available
        self.procedural_client = None
        self.obsidian_client = None
        if Client:
            try:
                self.procedural_client = Client("http://localhost:5002")  # Procedural MCP
                self.obsidian_client = Client("http://localhost:5001")  # Obsidian MCP
            except Exception as e:
                print(f"⚠️ Failed to initialize MCP clients: {e}")
        
        self.use_gemini = False
    
    def get_default_config(self):
        """Get default configuration"""
        return {
            'ollama': {
                'models': {
                    'story': 'llama2',
                    'analysis': 'llama2'
                },
                'temperature': {
                    'story': 0.8,
                    'analysis': 0.3
                }
            },
            'gemini': {
                'model': 'gemini-pro'
            },
            'godot': {
                'export_on_generate': False
            }
        }

    async def interact(self, user_input: str, mode: str = "story", use_gemini: bool = False) -> str:
        if not user_input.strip():
            return "Please provide input"
        
        context = self.build_context(user_input, mode)
        if user_input.startswith('/'):
            return await self.handle_command(user_input)
        
        if use_gemini or self.use_gemini:
            if genai and self.gemini_api_key:
                response = await self.call_gemini(user_input, context)
            else:
                return "⚠️ Gemini not available. Using Ollama instead."
                response = await self.call_ollama(user_input, context, mode)
        else:
            if self.ollama_client:
                response = await self.call_ollama(user_input, context, mode)
            else:
                return "⚠️ No AI service available. Please install Ollama or configure Gemini."
        
        self.session_history.append({"user": user_input, "response": response})
        self.save_universe_state()
        return response

    async def handle_command(self, command: str) -> str:
        if command.startswith('/story'):
            return await self.interact(command[7:], "story")
        elif command.startswith('/generate'):
            element_type = command.split()[1] if len(command.split()) > 1 else "character"
            return await self.procedural_generate(element_type, {})
        elif command == '/export':
            return await self.export_full_universe_to_godot()
        elif command == '/analyze':
            return json.dumps(await self.analyze_consistency(), indent=2)
        elif command == '/gemini':
            self.use_gemini = not self.use_gemini
            return f"Gemini toggled: {self.use_gemini}"
        elif command == '/quit':
            self.save_universe_state()
            exit(0)
        return "Unknown command"

    async def procedural_generate(self, element_type: str, constraints: Dict) -> str:
        if not self.procedural_client or not self.obsidian_client:
            return "⚠️ MCP clients not available for procedural generation"
        
        try:
            # Call MCP procedural server
            result = await self.procedural_client.call_tool("generate_" + element_type, constraints)
            # Assume result is TextContent, save to vault via obsidian MCP
            await self.obsidian_client.call_tool("create_note", {
                "folder": element_type.capitalize(), 
                "title": result.text.get("name"), 
                "metadata": result.text, 
                "content": result.text.get("description")
            })
            if self.config['godot']['export_on_generate']:
                await self.export_full_universe_to_godot()
            return json.dumps(result.text, indent=2)
        except Exception as e:
            return f"❌ Procedural generation failed: {e}"

    async def analyze_consistency(self) -> Dict:
        # Use analysis model to check vault
        notes = await self.obsidian_client.call_tool("search_notes", {"query": "*", "type": "*", "limit": 100})
        prompt = "Analyze consistency of universe: " + json.dumps(notes)
        response = await self.call_ollama(prompt, {}, "analysis")
        return {"report": response}

    async def export_full_universe_to_godot(self) -> str:
        # Use obsidian MCP to export
        result = await self.obsidian_client.call_tool("export_to_godot", {"export_type": "all"})
        return "Exported to Godot: " + str(result)

    def build_context(self, query: str, mode: str) -> Dict:
        # Build context from session history and vault
        context = {"history": self.session_history[-5:], "query": query}
        # Add relevant notes from vault
        notes = asyncio.run(self.obsidian_client.call_tool("search_notes", {"query": query, "limit": 3}))
        context["relevant_notes"] = notes
        return context

    async def call_ollama(self, prompt: str, context: Dict, mode: str) -> str:
        if not self.ollama_client:
            return "❌ Ollama client not available"
        
        try:
            model = self.config['ollama']['models'].get(mode, 'llama2')
            temperature = self.config['ollama']['temperature'].get(mode, 0.7)
            full_prompt = json.dumps(context) + "\n\n" + prompt
            response = await self.ollama_client.generate(
                model=model, 
                prompt=full_prompt, 
                options={"temperature": temperature}
            )
            return response.get('response', 'No response generated')
        except Exception as e:
            return f"❌ Ollama generation failed: {e}"

    async def call_gemini(self, prompt: str, context: Dict) -> str:
        if not genai or not self.gemini_api_key:
            return "❌ Gemini not available"
        
        try:
            model = genai.GenerativeModel(self.config['gemini']['model'])
            full_prompt = json.dumps(context) + "\n\n" + prompt
            response = model.generate_content(full_prompt)
            return response.text
        except Exception as e:
            return f"❌ Gemini generation failed: {e}"

    def save_universe_state(self):
        if not self.vault_path:
            return
        
        try:
            metadata_dir = os.path.join(self.vault_path, '_metadata')
            os.makedirs(metadata_dir, exist_ok=True)
            state_file = os.path.join(metadata_dir, 'universe_state.json')
            with open(state_file, 'w', encoding='utf-8') as f:
                json.dump({"history": self.session_history}, f, indent=2)
        except Exception as e:
            print(f"⚠️ Failed to save universe state: {e}")

async def main():
    try:
        engine = UniverseEngine()
        print("🌌 Storyverse Universe Engine started. Type /help for commands.")
        print("Available commands: /story, /generate, /export, /analyze, /gemini, /quit")
        
        while True:
            try:
                user_input = input("🌌 > ").strip()
                if not user_input:
                    continue
                
                response = await engine.interact(user_input)
                print(response)
            except KeyboardInterrupt:
                print("\n🛑 Shutting down...")
                engine.save_universe_state()
                break
            except EOFError:
                print("\n🛑 EOF received. Shutting down...")
                engine.save_universe_state()
                break
            except Exception as e:
                print(f"❌ Error: {e}")
                
    except Exception as e:
        print(f"❌ Failed to start engine: {e}")
        sys.exit(1)

if __name__ == "__main__":
    asyncio.run(main())