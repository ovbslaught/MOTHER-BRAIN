extends Node
# OMEGA-CORE-01: WIZ_ARCHITECT Godot-Side Bridge (UDP Listener)
# Attach this script to an Autoload singleton (e.g., "WizDM") in Godot 4.x

var server := UDPServer.new()
var listen_port := 4242

func _ready() -> void:
    var err = server.listen(listen_port)
    if err == OK:
        print("cat> [WIZ_DM] Listening for Python Overlord on UDP ", listen_port)
    else:
        printerr("cat> [ERROR] WIZ_DM failed to bind port.")

func _process(_delta: float) -> void:
    server.poll()
    if server.is_connection_available():
        var peer : PacketPeerUDP = server.take_connection()
        var pkt = peer.get_packet()
        var payload_str = pkt.get_string_from_utf8()
        
        var json = JSON.new()
        if json.parse(payload_str) == OK:
            var data = json.get_data()
            _execute_dm_command(data)

func _execute_dm_command(cmd: Dictionary) -> void:
    if not cmd.has("action"): return
    
    match cmd["action"]:
        "SPAWN_HAZARD":
            var entity = cmd.get("entity", "Unknown")
            var sector = cmd.get("sector", 0)
            print("cat> [WIZ_DM] Spawning Hazard: ", entity, " in Sector ", sector)
            # Instantiate logic here:
            # var hazard = preload("res://hazards/gristle_kin.tscn").instantiate()
            # add_child(hazard)
            
        "ALTER_GRAVITY":
            var new_g = cmd.get("value", 9.8)
            PhysicsServer3D.area_set_param(get_viewport().find_world_3d().space, PhysicsServer3D.AREA_PARAM_GRAVITY, new_g)
            print("cat> [WIZ_DM] Environmental Gravity shifted to: ", new_g)
            
        "SHAPE_REWARD":
            print("cat> [WIZ_DM] Adjusting RL reward heuristics based on Swarm performance.")
