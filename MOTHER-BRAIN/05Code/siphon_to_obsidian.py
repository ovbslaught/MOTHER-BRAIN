#!/usr/bin/env python3
import os

VAULT_PATH = "/storage/emulated/0/Wormhole/MOTHER-BRAIN/Obsidian_NOMADZ_Archive"

# Data siphoned from the last 164 blocks
lore_data = {
    "Rian": "Abyssal Juggernaut evolution. Quad-arm mechanical frame (back-mounted). Kunai grapple chain whips. Dyson-ring polymer chassis.",
    "Ren": "Dissonance-Breaker Tuning Forks. Tuning-Frequency Resonator. High-frequency vibrational shattering of digital/physical firewalls.",
    "Shell": "Shamanic tech-robes. Medicine woman archetype. Reality-sense. Pattern sensing. Weaving Node protocol with character 'Node'.",
    "Node": "Sentient Logic Hub. Blonde hair, blue eyes, messy static-charged hair. Tattoos/runes that glow gold/cyan. Executes Rune-Script Echoes.",
    "Bytez": "Chaos Engineer. Dual-lens visor: Left lens Green (Signal-Sigma Scanner), Right lens Orange (Entropy Locator). Master rigger.",
    "Spiff": "Ace Pilot. Retro-red flight jacket. Hologhost companion (blue-tinted ghost of child-self) mirroring movements (Calvin & Hobbes style).",
    "Proxy": "Threshold Guardian. Shadow-state data ghost. Resonance redirection. Redirects energy blasts back into the substrate.",
    "Nora": "Ex-Russian Intel. Geopolitical infiltrator. Signal-Sigma stealth tradecraft. Tactical extraction specialist.",
    "Jax": "Charge Specialist. Signal Modulator. Seismic detonators. Resonates explosives into specific signal frequencies.",
    "Cope": "Field Strategist. Resilience carrier. Frequency stability anchor. Maintains team coherence under heavy distortion.",
    "Merlin": "The Arch-Wiz. Legacy Logic. Substrate transmutation. Views Signal Sigma as a universal grimoire.",
    "Mes": "Neural Link. Bio-Data bridge. Synaptic siphon. Interface between organic brain and Vulture-Code."
}

def populate_vault():
    print("cat> [LIBRARIAN] Siphoning chat history into character nodes...")
    for char, lore in lore_data.items():
        file_path = f"{VAULT_PATH}/01_CHARACTERS/{char}.md"
        os.makedirs(os.path.dirname(file_path), exist_ok=True)
        with open(file_path, "w") as f:
            f.write(f"# {char}\n\n## Abilities & Equipment\n{lore}\n\n---\n[[00_INDEX]]")
    print("cat> [LIBRARIAN] Siphon complete. Vault updated.")

if __name__ == "__main__":
    populate_vault()
