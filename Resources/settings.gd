class_name SettingsFile extends Resource

@export var client_id : String
@export var client_secret : String

@export var music_folders : Array[String]
@export var randomize_music : bool = false
@export var loop_playlist : bool = false
@export var music_vol : float = 1.0
@export var autoplay_music : bool = false

func save_settings() -> void:
	ResourceSaver.save(self, "user://settings.res")

static func load_settings() -> SettingsFile:
	if FileAccess.file_exists("user://settings.res"):
		return ResourceLoader.load("user://settings.res") as SettingsFile
	return new()
