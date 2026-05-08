#!/usr/bin/env python3
# OMEGA-CORE-01: MP4-to-Dynamic-Noise-Matrix
# Extracts frames from NotebookLM MP4s to feed the Analog CRT Shaders.

import os
import subprocess

VIDEO_DIR = "/storage/emulated/0/Wormhole/NOMADZ-0/Media/NotebookLM"
FRAME_OUT = "/storage/emulated/0/Wormhole/NOMADZ-0/Assets/Shaders/NoiseFrames"
os.makedirs(FRAME_OUT, exist_ok=True)

def siphon_video_frames():
    print("cat> [SIPHON] Extracting fuzzy logic frames from MP4 substrate...")
    for file in os.listdir(VIDEO_DIR):
        if file.endswith(".mp4"):
            # Extracting 1 frame per second to create a low-fps 'fuzzy' loop
            input_path = os.path.join(VIDEO_DIR, file)
            output_pattern = os.path.join(FRAME_OUT, f"{file}_frame_%04d.jpg")
            subprocess.run([
                "ffmpeg", "-i", input_path, "-vf", "fps=1,scale=256:256", 
                "-q:v", "31", output_pattern
            ], capture_output=True)
            print(f"cat> [SIPHON] Siphoned: {file}")

if __name__ == "__main__":
    siphon_video_frames()
