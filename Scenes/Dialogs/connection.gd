extends MarginContainer

func _ready() -> void:
	%ClientId.text = Managers.settings.data.client_id
	%ClientSecret.text = Managers.settings.data.client_secret
	%ObsHost.text = Managers.settings.data.obs_host
	%ObsPort.text = str(Managers.settings.data.obs_port)
	%ObsPassword.text = Managers.settings.data.obs_pass
	
	if Managers.obs.is_connected_to_host():
		%ConnectStatus.text = "[color=green]Connected[/color]"
		var streaming := await Managers.obs.get_stream_status()
		var recording := await Managers.obs.get_record_status()
		if streaming.output_active:
			%StreamingStatus.text = "[color=green]Streaming[/color]"
		if recording.output_active:
			%RecordingStatus.text = "[color=green]Recording[/color]"
	
	Managers.obs.streaming_state.connect(_handle_stream_state)
	Managers.obs.recording_state.connect(_handle_record_state)
	
	%Authorize.pressed.connect(func():
		var id = %ClientId.text
		var secret = %ClientSecret.text
		
		if id == "" and secret == "":
			print("Need to provide a ID and Secret")
			return
		
		Managers.settings.data.client_id = id
		Managers.settings.data.client_secret = secret
		
		Managers.twitch.setup_auth_info(id, secret)
	)
	
	%ConnectOBS.pressed.connect(func():
		var host = %ObsHost.text
		var port = int(%ObsPort.text)
		var password = %ObsPassword.text
		Managers.obs.connection_ready.connect(_handle_save_obs)
		Managers.obs.connection_failed.connect(_handle_fail_obs)
		Managers.obs.connection_closed_clean.connect(_handle_fail_obs)
		Managers.obs.connect_to_host(host, port, password)
		%ConnectOBS.disabled = true
	)

func _handle_save_obs() -> void:
	Managers.settings.data.obs_host = %ObsHost.text
	Managers.settings.data.obs_port = int(%ObsPort.text)
	Managers.settings.data.obs_pass = %ObsPassword.text
	%ConnectStatus.text = "[color=green]Connected[/color]"
	Managers.obs.connection_ready.disconnect(_handle_save_obs)
	Managers.obs.connection_failed.disconnect(_handle_fail_obs)
	Managers.obs.connection_closed_clean.disconnect(_handle_fail_obs)

func _handle_fail_obs(code : int = -1, message : String = "Unable to connect to host.") -> void:
	%ConnectStatus.text = "[color=red]Failed to connect to OBS, (Reason: %s)[/color]" % message
	Managers.obs.connection_ready.disconnect(_handle_save_obs)
	Managers.obs.connection_failed.disconnect(_handle_fail_obs)
	Managers.obs.connection_closed_clean.disconnect(_handle_fail_obs)
	%ConnectOBS.disabled = false
	Managers.obs.reset_obs()

func _handle_stream_state(streaming : bool) -> void:
	if streaming:
		%StreamingStatus.text = "[color=green]Streaming[/color]"
	else:
		%StreamingStatus.text = "[color=red]Offline[/color]"

func _handle_record_state(recording : bool, _path : String) -> void:
	if recording:
		%RecordingStatus.text = "[color=green]Recording[/color]"
	else:
		%RecordingStatus.text = "[color=red]Not Recording[/color]"
