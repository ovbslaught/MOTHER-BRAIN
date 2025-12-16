extends Node2D

func _ready():
    if not UniverseDataLoader.is_loaded:
        yield(UniverseDataLoader, "data_loaded")
    load_available_quests()
    spawn_heroes()

func spawn_heroes():
    var heroes = UniverseDataLoader.get_characters_by_faction("heroes")
    for hero in heroes:
        UniverseDataLoader.spawn_character(hero["name"], Vector2(rand_range(0, 800), rand_range(0, 600)))

func load_available_quests():
    var quests = UniverseDataLoader.get_available_quests(1)  # Assume player level 1
    for quest in quests:
        print("Available quest: " + quest["title"])
        emit_signal("quest_available", quest)