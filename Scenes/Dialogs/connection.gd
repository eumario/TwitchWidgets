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
	
	if Managers.twitch.broadcaster != null:
		if Managers.twitch.broadcaster.auth.is_authenticated:
			%StreamerOnline.text = "[color=green]Authorized: %s[/color] - [color=red]Offline[/color]" % Managers.twitch.broadcaster.user.display_name
		else:
			%StreamerOnline.text = "[color=red]Authroized: null[/color] - [color=red]Offline[/color]"
	else:
		%StreamerOnline.text = "[color=red]Authroized: null[/color] - [color=red]Offline[/color]"
	
	if Managers.twitch.bot != null:
		if Managers.twitch.bot.auth.is_authenticated:
			%BotOnline.text = "[color=green]Authorized: %s[/color] - [color=red]Offline[/color]" % Managers.twitch.bot.user.display_name
		else:
			%BotOnline.text = "[color=red]Authroized: null[/color] - [color=red]Offline[/color]"
	else:
		%BotOnline.text = "[color=red]Authroized: null[/color] - [color=red]Offline[/color]"
	
	Managers.obs.streaming_state.connect(_handle_stream_state)
	Managers.obs.recording_state.connect(_handle_record_state)
	
	%AuthorizeStreamer.pressed.connect(func():
		var id = %ClientId.text
		var secret = %ClientSecret.text
		
		if id == "" and secret == "":
			print("Need to provide a ID and Secret")
			return
		
		if Managers.twitch.broadcaster == null:
			await Managers.twitch.setup_streamer_auth(id, secret)
			
		if Managers.twitch.broadcaster.auth.is_authenticated:
			Managers.settings.data.client_id = id
			Managers.settings.data.client_secret = secret
			await Managers.twitch.broadcaster.get_user()
			%StreamerOnline.text = "[color=green]Authorized: %s[/color] - [color=red]Offline[/color]" % Managers.twitch.broadcaster.user.display_name
			return
		
		if await Managers.twitch.broadcaster.authorize():
			%StreamerOnline.text = "[color=green]Authorized: %s[/color] - [color=red]Offline[/color]" % Managers.twitch.broadcaster.user.display_name
		else:
			%StreamerOnline.text = "[color=red]Authroized: null[/color] - [color=red]Offline[/color]"
	)
	%AuthorizeBot.pressed.connect(func():
		var id = %ClientId.text
		var secret = %ClientSecret.text
		
		if id == "" and secret == "":
			print("Need to provide a ID and Secret")
			return
		
		if Managers.twitch.bot == null:
			Managers.twitch.setup_bot_auth(id, secret)
			
		if Managers.twitch.bot.auth.is_authenticated:
			Managers.settings.data.client_id = id
			Managers.settings.data.client_secret = secret
			await Managers.twitch.bot.get_user()
			%BotOnline.text = "[color=green]Authorized: %s[/color] - [color=red]Offline[/color]" % Managers.twitch.bot.user.display_name
			return
		
		if await Managers.twitch.bot.authorize():
			%BotOnline.text = "[color=green]Authorized: %s[/color] - [color=red]Offline[/color]" % Managers.twitch.bot.user.display_name
		else:
			%BotOnline.text = "[color=red]Authroized: null[/color] - [color=red]Offline[/color]"
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

func _handle_fail_obs(_code : int = -1, message : String = "Unable to connect to host.") -> void:
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
