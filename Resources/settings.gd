class_name SettingsFile extends Resource

#region Twitch Connection
@export var client_id : String :
	set(value):
		if client_id != value:
			client_id = value
			emit_changed()
@export var client_secret : String :
	set(value):
		if client_secret != value:
			client_secret = value
			emit_changed()
#endregion

#region OBS Connection
@export var obs_host : String :
	set(value):
		if obs_host != value:
			obs_host = value
			emit_changed()

@export var obs_port : int :
	set(value):
		if obs_port != value:
			obs_port = value
			emit_changed()

@export var obs_pass : String :
	set(value):
		if obs_pass != value:
			obs_pass = value
			emit_changed()
#endregion

#region Music Settings
@export var music_folders : Array[String] :
	set(value):
		if music_folders != value:
			music_folders = value
			emit_changed()
@export var randomize_music : bool :
	set(value):
		if randomize_music != value:
			randomize_music = value
			emit_changed()
@export var loop_playlist : bool :
	set(value):
		if loop_playlist != value:
			loop_playlist = value
			emit_changed()
@export var music_vol : float :
	set(value):
		if music_vol != value:
			music_vol = value
			emit_changed()
@export var autoplay_music : bool :
	set(value):
		if autoplay_music != value:
			autoplay_music = value
			emit_changed()
#endregion

#region Scene Collection & Alert Set
@export var collection_name : String :
	set(value):
		if collection_name != value:
			collection_name = value
			emit_changed()	
@export var alert_set_name : String :
	set(value):
		if alert_set_name != value:
			alert_set_name = value
			emit_changed()
#endregion

#region Startup
@export var auto_connect_twitch : bool:
	set(value):
		if auto_connect_twitch != value:
			auto_connect_twitch = value
			emit_changed()
@export var auto_connect_obs : bool:
	set(value):
		if auto_connect_obs != value:
			auto_connect_obs = value
			emit_changed()
@export var start_fullscreen : bool:
	set(value):
		if start_fullscreen != value:
			start_fullscreen = value
			emit_changed()
@export var use_screen : bool:
	set(value):
		if use_screen != value:
			use_screen = value
			emit_changed()
@export var selected_screen : int:
	set(value):
		if selected_screen != value:
			selected_screen = value
			emit_changed()
@export var log_debug : bool:
	set(value):
		if log_debug != value:
			log_debug = value
			emit_changed()
@export var log_to_console : bool:
	set(value):
		if log_to_console != value:
			log_to_console = value
			emit_changed()
#endregion

func _init() -> void:
	music_vol = 1.0

func save_settings() -> void:
	ResourceSaver.save(self, "user://settings.res")

static func load_settings() -> SettingsFile:
	if FileAccess.file_exists("user://settings.res"):
		return ResourceLoader.load("user://settings.res") as SettingsFile
	return new()

var _stage : SettingsFile

func snapshot() -> void:
	_stage = duplicate(true)

func restore() -> void:
	if _stage == null: return
	for prop in get_property_list():
		if prop.usage & PROPERTY_USAGE_SCRIPT_VARIABLE == PROPERTY_USAGE_SCRIPT_VARIABLE:
			set(prop.name, _stage.get(prop.name))
	_stage = null
	emit_changed()
