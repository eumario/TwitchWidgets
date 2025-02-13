class_name ObsManager extends Node

var connection : NoOBSWS

#region Base Signals
signal connection_ready()
signal connection_failed()
signal connection_closed_clean()
signal error(message : String)
signal scene_changed(scene_name : String)
signal streaming_state(live : bool)
signal recording_state(recording : bool, path : String)
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

func get_current_program_scene() -> String:
	var res := connection.make_generic_request("GetCurrentProgramScene", {})
	await res.response_received
	Logger.debug("ObsManager: GetCurrentProgramScene() -> %s" % res.message.response_data.scene_name)
	return res.message.response_data.scene_name

func get_stream_status() -> StreamStatus:
	var res := connection.make_generic_request("GetStreamStatus", {})
	await res.response_received
	Logger.debug("ObsManager: GetStreamStatus()")
	#var data := NoOBSWS.Message.camel_to_snake_recursive(res.message.response_data)
	return StreamStatus.new(res.message.response_data)

func get_record_status() -> RecordStatus:
	var res := connection.make_generic_request("GetRecordStatus", {})
	await res.response_received
	Logger.debug("ObsManager: GetRecordStatus()")
	#var data := NoOBSWS.Message.camel_to_snake_recursive(res.message.response_data)
	return RecordStatus.new(res.message.response_data)
#endregion

#region Signal Handlers
func _handle_event(event : NoOBSWS.Message) -> void:
	var data = event.get_data()
	
	match data.event_type:
		"CurrentProgramSceneChanged":
			scene_changed.emit(data.event_data.scene_name)
			Logger.debug("ObsManager: CurrentProgramSceneChanged('%s')" % data.event_data.scene_name)
		"StreamStateChanged":
			if data.event_data.output_state == "OBS_WEBSOCKET_OUTPUT_STARTED" or \
				data.event_data.output_state == "OBS_WEBSOCKET_OUTPUT_STOPPED":
				streaming_state.emit(data.event_data.output_active)
				Logger.debug("ObsManager: StreamStateChanged(%s)" % data.event_data.output_active)
		"RecordStateChanged":
			if data.event_data.output_state == "OBS_WEBSOCKET_OUTPUT_STARTED" or \
				data.event_data.output_state == "OBS_WEBSOCKET_OUTPUT_STOPPED":
				recording_state.emit(data.event_data.output_active, data.event_data.output_path)
				Logger.debug("ObsManager: RecordStateChanged(%s, %s)" % [data.event_data.output_active, data.event_data.output_path])
#endregion

#region Support Classes
class StreamStatus extends RefCounted:
	var output_active : bool
	var output_reconnecting : bool
	var output_timecode : String
	var output_duration : int
	var output_congestion : int
	var output_bytes : int
	var output_skipped_frames : int
	var output_total_frames : int
	
	func _init(d : Dictionary = {}) -> void:
		for i in d:
			if i in self:
				self[i] = d[i]

class RecordStatus extends RefCounted:
	var output_active : bool
	var output_paused : bool
	var output_timecode : String
	var output_duration : int
	var output_bytes : int
	
	func _init(d : Dictionary = {}) -> void:
		for i in d:
			if i in self:
				self[i] = d[i]
#endregion
