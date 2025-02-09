extends Node

var settings : SettingsManager
var twitch : TwitchManager
var streamdeck : StreamDeckManager
var obs : ObsManager
var music : MusicManager
var tts : TtsManager
var scene : SceneManager
var alert : AlertManager

func _ready() -> void:
	settings = SettingsManager.new()
	settings.name = "SettingsManager"
	add_child(settings)
	Logger.info("Loaded SettingsManager.")
	
	Logger.new("user://logs/widgets.log")
	Logger.instance.log_level = Logger.LogLevel.ERROR
	
	twitch = TwitchManager.new()
	twitch.name = "TwitchManager"
	add_child(twitch)
	Logger.info("Loaded TwitchManager.")
	
	streamdeck = StreamDeckManager.new()
	streamdeck.name = "StreamDeckManager"
	add_child(streamdeck)
	Logger.info("Loaded StreamDeckManager.")

	obs = ObsManager.new()
	obs.name = "ObsManager"
	add_child(obs)
	Logger.info("Loaded ObsManager.")

	music = MusicManager.new()
	music.name = "MusicManager"
	add_child(music)
	Logger.info("Loaded MusicManager.")

	tts = TtsManager.new()
	tts.name = "TtsManager"
	add_child(tts)
	Logger.info("Loaded TtsManager.")
	
	scene = SceneManager.new()
	scene.name = "SceneManager"
	add_child(scene)
	Logger.info("Loaded SceneManager.")
	
	alert = AlertManager.new()
	alert.name = "AlertManager"
	add_child(alert)
	Logger.info("Loaded AlertManager.")
