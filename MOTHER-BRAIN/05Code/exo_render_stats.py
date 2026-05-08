#!/usr/bin/env python3
# OMEGA-CORE-01: NOMADZ Exosuit Performance Metrics

def get_rian_specs():
    specs = {
        "Model": "RIAN-01-WHIPMASTER",
        "Appendages": "4x Hydraulic Arms (Back-mounted)",
        "Weaponry": "Kunai Grapple Chain (High-Tension)",
        "Reach": "25 Meters",
        "Torque": "5000Nm per Arm"
    }
    print("cat> [ARMORY] Rian's Exosuit Specs Loaded.")
    for key, val in specs.items():
        print(f"  {key}: {val}")

if __name__ == "__main__":
    get_rian_specs()
