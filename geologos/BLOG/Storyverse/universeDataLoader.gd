extends Node

signal data_loaded
signal character_spawned(character_data)
signal location_loaded(location_data)
signal quest_available(quest_data)

var characters: Dictionary = {}
var locations: Dictionary = {}
var quests: Dictionary = {}
var items: Dictionary = {}
var factions: Dictionary = {}
var is_loaded: bool = false

func _ready():
    reload_data()

func reload_data():
    var data = load_json("res://data/universe.json")
    if data:
        _load_characters(data.get("characters", []))
        _load_locations(data.get("locations", []))
        _load_quests(data.get("quests", []))
        _load_items(data.get("items", []))
        _load_factions(data.get("factions", []))
        is_loaded = true
        emit_signal("data_loaded")

func load_json(path: String) -> Dictionary:
    var file = File.new()
    if file.open(path, File.READ) != OK:
        return {}
    var text = file.get_as_text()
    file.close()
    var parse_result = JSON.parse(text)
    if parse_result.error != OK:
        print("JSON Parse Error: ", parse_result.error_string)
        return {}
    return parse_result.result

func _load_characters(char_array: Array):
    characters.clear()
    for char in char_array:
        characters[char["name"].to_lower().replace(" ", "_")] = char

# Similar _load methods for locations, quests, items, factions

func get_character(id: String) -> Dictionary:
    return characters.get(id.to_lower().replace(" ", "_"), {})

func get_location(id: String) -> Dictionary:
    return locations.get(id.to_lower().replace(" ", "_"), {})

func get_quest(id: String) -> Dictionary:
    return quests.get(id.to_lower().replace(" ", "_"), {})

func get_faction(id: String) -> Dictionary:
    return factions.get(id.to_lower().replace(" ", "_"), {})

func get_characters_by_faction(name: String) -> Array:
    return characters.values().filter(func(c): return c.get("faction") == name)

func get_available_quests(player_level: int) -> Array:
    return quests.values().filter(func(q): return q.get("required_level", 0) <= player_level and q.get("status") == "available")

func get_locations_by_type(type: String) -> Array:
    return locations.values().filter(func(l): return l.get("location_type") == type)

func spawn_character(id: String, position: Vector2):
    var char_data = get_character(id)
    if char_data:
        # Assume a CharacterScene prefab exists
        var scene = load("res://scenes/Character.tscn").instance()
        scene.position = position
        scene.setup_from_data(char_data)
        get_tree().current_scene.add_child(scene)
        emit_signal("character_spawned", char_data)

func complete_quest(id: String):
    var quest = get_quest(id)
    if quest:
        quest["status"] = "completed"
        # Save state
        save_state_to_file("res://data/universe.json")

func discover_location(id: String):
    var loc = get_location(id)
    if loc:
        loc["discovered"] = true
        save_state_to_file("res://data/universe.json")

func update_relationship(char1: String, char2: String, type: String, strength: int):
    var c1 = get_character(char1)
    if c1:
        c1["relationships"].append({"character": char2, "type": type, "strength": strength})
        save_state_to_file("res://data/universe.json")

func save_state_to_file(path: String):
    var file = File.new()
    file.open(path, File.WRITE)
    file.store_string(JSON.print({
        "characters": characters.values(),
        "locations": locations.values(),
        "quests": quests.values(),
        "items": items.values(),
        "factions": factions.values()
    }))
    file.close()