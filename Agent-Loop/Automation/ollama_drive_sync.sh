#!/bin/bash
rclone sync gdrive:Models $HOME/sdcard/ollama_models --update
ollama list
