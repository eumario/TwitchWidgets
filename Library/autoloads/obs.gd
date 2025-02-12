class_name ObsManager extends Node

var connection : NoOBSWS

#region Base Signals
signal connection_ready()
signal connection_failed()
signal connection_closed_clean()
signal error(message : String)
#endregion

#region OBSManager Signals
#endregion

#region Private Variables
var _connected : bool = false
#endregion

#region Godot Overrides
func _ready() -> void:
	connection = NoOBSWS.new()
	connection.name = "NoOBSWS"
	add_child(connection)
	# Internal Signal Connections
	connection.connection_ready.connect(func(): _connected = true)
	connection.connection_failed.connect(func(): _connected = false)
	connection.connection_closed_clean.connect(func(_i, _j): _connected = false)
	connection.event_received.connect(_handle_event)
	
	# Repeat Signal connections
	connection.connection_ready.connect(connection_ready.emit)
	connection.connection_failed.connect(connection_failed.emit)
	connection.error.connect(error.emit)
	connection.connection_closed_clean.connect(connection_closed_clean.emit)
	Managers.init_finished.connect(func():
		var settings = Managers.settings.data
		if settings.auto_connect_obs:
			connection.connect_to_obsws(settings.obs_host, settings.obs_port, settings.obs_pass)
	)
#endregion

#region Public API
func connect_to_host(host : String, port : int, obs_pass : String) -> void:
	connection.connect_to_obsws(host, port, obs_pass)

func is_connected_to_host() -> bool:
	return _connected

func reset_obs() -> void:
	remove_child(connection)
	connection.queue_free()
	_ready()
#endregion

#region Signal Handlers
func _handle_event(event : NoOBSWS.Message) -> void:
	var data = event.get_data()
	print(data)
	match data.event_type:
		"CurrentProgramSceneChanged":
			Managers.scene.scene_changed(data.event_data.scene_name)
#endregion
