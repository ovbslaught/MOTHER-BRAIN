#!/usr/bin/env python3
# OMEGA-CORE-01: VULTURE-BRAIN Parallel PPO Training Loop
# Utilizes SubprocVecEnv to run multiple Godot instances simultaneously.

import os
import sys
import argparse
import logging
from stable_baselines3 import PPO
from stable_baselines3.common.vec_env import SubprocVecEnv, VecNormalize
from stable_baselines3.common.monitor import Monitor
from stable_baselines3.common.callbacks import CheckpointCallback
from godot_rl.wrappers.stable_baselines_wrapper import StableBaselinesGodotEnv

# Paths
BASE_DIR = "/storage/emulated/0/Wormhole/MOTHER-BRAIN"
CODE_DIR = os.path.join(BASE_DIR, "05Code")
MODEL_DIR = os.path.join(CODE_DIR, "models")
LOG_DIR = os.path.join(CODE_DIR, "tensorboard_logs")

# Godot Headless Export Path (Required for parallel execution)
# You must export your Godot project as a Linux/Android binary for this to work.
DEFAULT_GODOT_BIN = os.path.join(CODE_DIR, "exports", "vulture_headless.pck")

def make_env(env_path: str, port: int, rank: int):
    """Utility function to initialize multiple isolated Godot environments."""
    def _init():
        logging.info(f"cat> [MR.WIZ] Spawning Godot instance on TCP {port}...")
        # Godot-RL will launch the binary at env_path and connect to the specified port
        env = StableBaselinesGodotEnv(env_path=env_path, port=port, seed=rank)
        return Monitor(env)
    return _init

def train_parallel(args):
    logging.info(f"cat> [MR.WIZ] Constructing SubprocVecEnv with {args.n_envs} parallel instances...")
    
    if not os.path.exists(args.env_path) and args.env_path != "None":
        logging.warning(f"cat> [WARNING] Godot binary not found at {args.env_path}.")
        logging.warning("cat> Parallel training requires an exported headless Godot project.")
    
    # Map out the environment initialization functions
    env_fns = [make_env(args.env_path, args.base_port + i, i) for i in range(args.n_envs)]
    
    # SubprocVecEnv forks the processes to utilize multiple CPU cores
    vec_env = SubprocVecEnv(env_fns, start_method="fork")
    vec_env = VecNormalize(vec_env, norm_obs=True, norm_reward=True, clip_obs=10.)
    
    # Initialize PPO Policy
    model = PPO(
        "MultiInputPolicy", 
        vec_env, 
        learning_rate=0.0003,
        n_steps=2048,
        batch_size=64 * args.n_envs, # Scale batch size with number of environments
        n_epochs=10,
        gamma=0.99,
        gae_lambda=0.95,
        clip_range=0.2,
        ent_coef=0.005,
        verbose=1,
        tensorboard_log=LOG_DIR
    )

    checkpoint_callback = CheckpointCallback(
        save_freq=10000 // args.n_envs, 
        save_path=MODEL_DIR, 
        name_prefix="vulture_parallel_weights"
    )

    logging.info("cat> [MR.WIZ] Parallel swarm initialized. Commencing accelerated training loop...")
    
    try:
        model.learn(total_timesteps=args.steps, callback=checkpoint_callback)
    except KeyboardInterrupt:
        logging.info("cat> [MR.WIZ] Architect interrupt detected. Halting swarm...")
    finally:
        final_path = os.path.join(MODEL_DIR, "vulture_parallel_final")
        model.save(final_path)
        vec_env.save(os.path.join(MODEL_DIR, "vec_normalize_parallel.pkl"))
        vec_env.close()
        logging.info("cat> [MR.WIZ] Parallel VULTURE-BRAIN weights secured.")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="VULTURE-BRAIN Parallel PPO Trainer")
    parser.add_argument("--steps", type=int, default=1000000, help="Total timesteps to train")
    parser.add_argument("--n_envs", type=int, default=4, help="Number of parallel Godot instances")
    parser.add_argument("--base_port", type=int, default=10008, help="Starting TCP Port")
    parser.add_argument("--env_path", type=str, default=DEFAULT_GODOT_BIN, help="Path to exported Godot binary")
    args = parser.parse_args()

    logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(message)s')
    train_parallel(args)
