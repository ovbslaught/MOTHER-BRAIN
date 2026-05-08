extends Node
# OMEGA-CORE-01: VULTURE-BRAIN Godot Agent Component
# Provides mathematical state vectors and reward signals to PyTorch.
# Requires Godot RL Agents AIController3D to be in the project tree.

@export var agent_id: int = 0
var done: bool = false
var reward: float = 0.0

func _physics_process(delta: float) -> void:
    # 1. Reset reward accumulator per physical step
    reward = 0.0
    
    # 2. Add baseline mathematical heuristics (e.g., survival decay, velocity delta)
    # reward -= 0.01 * delta

func get_obs() -> Dictionary:
    # Sensor Data Matrix (State Space)
    # Must mathematically map to the MultiInputPolicy dimensions in SB3
    var parent = get_parent()
    var obs_vector = [
        parent.global_transform.origin.x,
        parent.global_transform.origin.y,
        parent.global_transform.origin.z,
        parent.linear_velocity.x,
        parent.linear_velocity.y,
        parent.linear_velocity.z
    ]
    return {"observation": obs_vector}

func get_reward() -> float:
    # Reward shaping algorithm output
    return reward

func get_action_space() -> Dictionary:
    # Action Matrix Definition
    # Continuous limits for physical motor outputs (-1.0 to 1.0)
    return {
        "motor_outputs": {
            "size": 3, 
            "action_type": "continuous"
        }
    }

func set_action(action: Dictionary) -> void:
    # Execute the PPO algorithm's numerical policy decision
    var motor_data = action["motor_outputs"]
    var parent = get_parent()
    
    # Apply raw numerical tensors to the physics body
    var force_vector = Vector3(motor_data[0], motor_data[1], motor_data[2])
    parent.apply_central_force(force_vector * 10.0)

func get_done() -> bool:
    var current_done = done
    done = false # Reset state tensor after transmission
    return current_done
