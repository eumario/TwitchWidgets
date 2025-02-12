class_name StreamDeckManager extends Node

#region Signals
signal settings()
signal shutdown()

signal music_volume(action : MusicVolume)
signal music_play()
signal music_pause()
signal music_next()

#endregion

#region Enums
enum MusicVolume { UP, DOWN, MUTE }
#endregion

#region Private Variables
var _streamdeck : SCWebsocketClient
#endregion

#region Godot Overrides
func _ready() -> void:
	_streamdeck = SCWebsocketClient.new()
	_streamdeck.processor = GodotProcessor.new()
	_streamdeck.processor.on_key_down.connect(_handle_signal)
	_streamdeck.deck_ready.connect(func(): Logger.info("Connected to StreamDeck."))
	_streamdeck.deck_closed.connect(func():
		Logger.warn("Stream Deck Closed, waiting half a second, and connecting again...")
		await get_tree().create_timer(3).timeout
		Logger.info("Attempting connection again...")
		_streamdeck.connect_to_url()
	)
	add_child(_streamdeck)
	Managers.init_finished.connect(_streamdeck.connect_to_url)
#endregion

#region Private Support Functions
#endregion

#region Signal Handlers
func _handle_signal(args : String) -> void:
	var args_arr := args.split(" ")
	match args_arr[0]:
		"settings":
			settings.emit()
		"shutdown":
			shutdown.emit()
		"music":
			if args_arr.has("vol_up"): music_volume.emit(MusicVolume.UP)
			if args_arr.has("vol_down"): music_volume.emit(MusicVolume.DOWN)
			if args_arr.has("mute"): music_volume.emit(MusicVolume.MUTE)
			if args_arr.has("pause"): music_pause.emit()
			if args_arr.has("play"): music_play.emit()
			if args_arr.has("next"): music_next.emit()
			
#endregion
