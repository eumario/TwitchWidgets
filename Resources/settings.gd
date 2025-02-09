class_name SettingsFile extends Resource

@export var client_id : String
@export var client_secret : String

func save_settings() -> void:
	ResourceSaver.save(self, "user://settings.res")

static func load_settings() -> SettingsFile:
	if FileAccess.file_exists("user://settings.res"):
		return ResourceLoader.load("user://settings.res") as SettingsFile
	return new()
