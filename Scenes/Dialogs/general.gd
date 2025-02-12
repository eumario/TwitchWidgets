extends MarginContainer

func _ready() -> void:
	_load_settings()
	
	#-- Signal Handlers --#
	%SceneSelection.item_selected.connect(func(i : int):
		if %SceneSelection.get_item_text(i) == "<None Selected>":
			Managers.settings.data.collection_name = ""
		else:
			Managers.settings.data.collection_name = %SceneSelection.get_item_text(i)
	)
	%AlertSelection.item_selected.connect(func(i : int):
		if %AlertSelection.get_item_text(i) == "<None Selected>":
			Managers.settings.data.alert_set_name = ""
		else:
			Managers.settings.data.alert_set_name = %AlertSelection.get_item_text(i)
	)
	
	# Music
	%FolderList.item_selected.connect(_handle_folder_selected)
	%FolderBrowse.pressed.connect(_handle_folder_browse)
	%AddFolder.pressed.connect(_handle_add_folder)
	%SaveFolder.pressed.connect(_handle_save_folder)
	%RemoveFolder.pressed.connect(_handle_remove_folder)
	%RandomizePlaylist.pressed.connect(func(): Managers.settings.data.randomize_music = %RandomizePlaylist.button_pressed)
	%LoopPlaylist.pressed.connect(func(): Managers.settings.data.loop_playlist = %LoopPlaylist.button_pressed)
	
	# Other Settings
	%AutoTwitch.pressed.connect(func(): Managers.settings.data.auto_connect_twitch = %AutoTwitch.button_pressed)
	%AutoObs.pressed.connect(func(): Managers.settings.data.auto_connect_obs = %AutoObs.button_pressed)
	%AutoMusic.pressed.connect(func(): Managers.settings.data.autoplay_music = %AutoMusic.button_pressed)
	%StartFullscreen.pressed.connect(func(): Managers.settings.data.start_fullscreen = %StartFullscreen.button_pressed)
	%UseScreen.pressed.connect(func():
		Managers.settings.data.use_screen = %UseScreen.button_pressed
		%ScreenSelect.disabled = !%UseScreen.button_pressed
	)
	%ShowDebug.pressed.connect(func(): Managers.settings.data.log_debug = %ShowDebug.button_pressed)
	%LogToConsole.pressed.connect(func(): Managers.settings.data.log_to_console = %LogToConsole.button_pressed)
	

#region Loading Settings
func _load_settings() -> void:
	#-- Scene Collection --#
	%SceneSelection.add_item("<None Selected>")
	for item : SceneCollection in Managers.scene.get_scene_collections():
		%SceneSelection.add_item(item.collection_name)
		if item.collection_name == Managers.settings.data.collection_name:
			%SceneSelection.select(%SceneSelection.item_count - 1)
	
	#-- Alert Sets --#
	%AlertSelection.add_item("<None Selected>")
	for item : AlertSet in Managers.alert.get_alert_sets():
		%AlertSelection.add_item(item.set_name)
		if item.set_name == Managers.settings.data.alert_set_name:
			%AlertSelection.select(%AlertSelection.item_count - 1)
	
	#-- Startup --#
	%AutoTwitch.button_pressed = Managers.settings.data.auto_connect_twitch
	%AutoObs.button_pressed = Managers.settings.data.auto_connect_obs
	%AutoMusic.button_pressed = Managers.settings.data.autoplay_music
	%StartFullscreen.button_pressed = Managers.settings.data.start_fullscreen
	%UseScreen.button_pressed = Managers.settings.data.use_screen
	%ScreenSelect.disabled = !Managers.settings.data.use_screen
	for screen in DisplayServer.get_screen_count():
		%ScreenSelect.add_item("Screen %d" % (screen + 1))
		if screen == Managers.settings.data.selected_screen:
			%ScreenSelect.select(screen)
	%ShowDebug.button_pressed = Managers.settings.data.log_debug
	%LogToConsole.button_pressed = Managers.settings.data.log_to_console
	
	#-- Music Player --#
	for folder in Managers.settings.data.music_folders:
		%FolderList.add_item(folder)
	
	%RandomizePlaylist.button_pressed = Managers.settings.data.randomize_music
	%LoopPlaylist.button_pressed = Managers.settings.data.loop_playlist
#endregion

#region Signal Handlers
func _handle_folder_browse() -> void:
	DisplayServer.file_dialog_show("Select folder with Music", OS.get_system_dir(OS.SYSTEM_DIR_MUSIC), "", false, DisplayServer.FILE_DIALOG_MODE_OPEN_DIR, [],
		func(status : bool, selected_paths : PackedStringArray, _index : int):
			if status:
				%FolderPath.text = selected_paths[0]
	)

func _handle_add_folder() -> void:
	var data = %FolderPath.text
	%FolderPath.text = ""
	%FolderList.add_item(data)
	%FolderList.deselect_all()
	%SaveFolder.disabled = true
	%RemoveFolder.disabled = true
	Managers.settings.data.music_folders.append(data)

func _handle_folder_selected(index : int) -> void:
	var path = %FolderList.get_item_text(index)
	%FolderPath.text = path
	%SaveFolder.disabled = false
	%RemoveFolder.disabled = false

func _handle_save_folder() -> void:
	var index = %FolderList.get_selected_items()[0]
	var data = %FolderPath.text
	%FolderPath.text = ""
	%FolderList.set_item_text(index, data)
	Managers.settings.data.music_folders[index] = data

func _handle_remove_folder() -> void:
	var index = %FolderList.get_selected_items()[0]
	var data = %FolderList.get_item_text(index)
	var dlg = ConfirmationDialog.new()
	dlg.title = "Directory Remove Confirm"
	dlg.dialog_text = "Are you sure that you want to remove '%s' from the list folders?" % data
	dlg.dialog_autowrap = true
	dlg.canceled.connect(dlg.queue_free)
	dlg.close_requested.connect(dlg.queue_free)
	dlg.confirmed.connect(func():
		%FolderList.remove_item(index)
		Managers.settings.data.music_folders.remove_at(index)
	)
	dlg.popup_exclusive_centered(self, Vector2i(200,150))

#endregion
