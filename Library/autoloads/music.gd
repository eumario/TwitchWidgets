class_name MusicManager extends Node

#region Signals
signal song_changed()
signal pause_changed()
#endregion

#region Private Variables
var _bus : int
var _player : AudioStreamPlayer
var _songList : Array[String] = []
var _playlist : Array[String] = []
var _currSong : String = ""
var _curVol : float = 1.0
var _oldVol : float = 0.0

var _cacheSongs : Dictionary[String, AudioStream] = {}
var _tags : Dictionary[String, MP3ID3Tag] = {}
#endregion

#region Public Properties
var current_song_tag : MP3ID3Tag
var current_song_index : int = -1
#endregion

#region Godot Overrides
func _ready() -> void:
	_bus = AudioServer.get_bus_index("music")
	_player = AudioStreamPlayer.new()
	_player.name = "AudioStreamPlayer"
	_player.bus = &"music"
	add_child(_player)
	
	Managers.settings.updated.connect(_handle_updated_settings)
	Managers.streamdeck.music_play.connect(_player.play)
	Managers.streamdeck.music_pause.connect(func():
		_player.stream_paused = !_player.stream_paused
		pause_changed.emit()
	)
	Managers.streamdeck.music_next.connect(_handle_player_finished)
	Managers.streamdeck.music_volume.connect(_handle_music_volume)
	_player.finished.connect(_handle_player_finished)
	
	Managers.init_finished.connect(_generate_songs)
#endregion

#region Public API
func get_spectrum() -> AudioEffectSpectrumAnalyzerInstance:
	return AudioServer.get_bus_effect_instance(_bus, 0) as AudioEffectSpectrumAnalyzerInstance

func is_playing() -> bool:
	return _player.playing

func get_playlist_size() -> int:
	return _playlist.size()

func get_current_track_length() -> float:
	if _currSong == "":
		return 0.0
	return _player.stream.get_length()

func get_current_track_time() -> float:
	if _currSong == "":
		return 0.0
	return _player.get_playback_position()

func get_current_album_art() -> Texture2D:
	if _currSong == "" or current_song_tag == null:
		return load("res://Plugins/cdwoverlay/Assets/Images/Icons/NoAlbumArt.png")
	
	var img = current_song_tag.getAttachedPicture()
	if img == null:
		return load("res://Plugins/cdwoverlay/Assets/Images/Icons/NoAlbumArt.png")
	return ImageTexture.create_from_image(img)

func get_current_album() -> String:
	if _currSong == "" or current_song_tag == null:
		return "Unknown Album"
	
	return current_song_tag.getAlbum()

func get_current_artist() -> String:
	if _currSong == "" or current_song_tag == null:
		return "Unknown Artist"
	
	return current_song_tag.getArtist()

func get_current_title() -> String:
	if _currSong == "": return "Unknown Song"
	if current_song_tag == null: return _currSong.get_basename()
	return current_song_tag.getTrackName()

func reset_music(play : bool = false) -> void:
	_currSong = ""
	current_song_index = -1
	if play:
		_handle_player_finished()

func change_volume(vol : float) -> void:
	var newVol := clampf(_curVol + vol, 0.0, 1.0)
	Logger.debug("MusicManager: Volume Changed: %f -> %s (Change: %f)" % [_curVol, newVol, vol])
	_curVol = newVol;
	
	Managers.settings.data.music_vol = newVol
	Managers.settings.save_settings(false)
	
	update_volume()

func update_volume() -> void:
	AudioServer.set_bus_volume_db(_bus, linear_to_db(_curVol))
#endregion

#region Private Support Functions
func _load_song(song : String) -> void:
	if song.ends_with(".mp3"):
		var mp3 = AudioStreamMP3.new()
		mp3.data = FileAccess.get_file_as_bytes(song)
		_cacheSongs[song] = mp3
		var tag = MP3ID3Tag.new()
		tag.stream = mp3
		_tags[song] = tag
	if song.ends_with(".wav"):
		var wav = AudioStreamWAV.new()
		wav.data =FileAccess.get_file_as_bytes(song)
		_cacheSongs[song] = wav
		_tags[song] = null
	if song.ends_with(".ogg"):
		var ogg = AudioStreamOggVorbis.load_from_file(song)
		_cacheSongs[song] = ogg
		_tags[song] = null

func _generate_songs() -> void:
	_songList.clear()
	_cacheSongs.clear()
	
	for folder in Managers.settings.data.music_folders:
		var files := _get_all_files(folder)
		for file in files.filter(func(x): return x.ends_with(".mp3") or x.ends_with(".ogg") or x.ends_with(".wav")):
			_songList.append(file)
	
	_playlist.clear()
	_playlist.assign(_songList.duplicate(true))
	if Managers.settings.data.randomize_music:
		_playlist.shuffle()
	_curVol = Managers.settings.data.music_vol
	AudioServer.set_bus_volume_db(_bus, linear_to_db(_curVol))
	if Managers.settings.data.autoplay_music:
		_handle_player_finished()

func _get_all_files(path : String) -> Array[String]:
	var files = PackedStringArray(Array(DirAccess.get_files_at(path)).map(func(x): return path.path_join(x)))
	for dir in DirAccess.get_directories_at(path):
		files.append_array(_get_all_files(path.path_join(dir)))
	var final_files : Array[String] = []
	final_files.assign(files)
	return final_files
#endregion

#region Signal Callbacks
func _handle_updated_settings() -> void:
	if is_playing():
		_player.stop()
	Logger.debug("MusicManager: Settings Updated, Generating Songs...")
	_generate_songs()

func _handle_player_finished() -> void:
	current_song_index += 1
	if current_song_index >= _songList.size():
		if Managers.settings.data.loop_playlist:
			current_song_index = 0
			if Managers.settings.data.randomize_music:
				_playlist.shuffle()
		else:
			return
	_currSong = _playlist[current_song_index]
	
	if not _cacheSongs.has(_currSong):
		_load_song(_currSong)
	
	_player.stream = _cacheSongs[_currSong]
	current_song_tag = _tags[_currSong]
	Logger.debug("MusicManager: PlayNextSong('%s')" % [_currSong])
	song_changed.emit()
	_player.play()

func _handle_music_volume(state : StreamDeckManager.MusicVolume) -> void:
	match state:
		StreamDeckManager.MusicVolume.UP:
			change_volume(0.05)
		StreamDeckManager.MusicVolume.DOWN:
			change_volume(-0.05)
		StreamDeckManager.MusicVolume.MUTE:
			if _oldVol == 0.0:
				_oldVol = _curVol
				_curVol = 0.0
			else:
				_curVol = _oldVol
				_oldVol = 0.0
			update_volume()
	pass
#endregion
