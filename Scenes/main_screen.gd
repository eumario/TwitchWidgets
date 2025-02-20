class_name MainScreen extends Control

const NO_SCENE = preload("res://Scenes/NoSceneSelected.tscn")
var _current_scene : Control
var _next_scene : Control
var _switching : bool = false

func _ready() -> void:
	var scene = NO_SCENE.instantiate()
	add_child(scene)
	_current_scene = scene
	Managers.streamdeck.settings.connect(_handle_settings)
	Managers.streamdeck.shutdown.connect(_handle_shutdown)
	Managers.scene.main_window = self
	if Managers.settings.data.use_screen:
		get_window().current_screen = Managers.settings.data.selected_screen
	
	if Managers.settings.data.start_fullscreen:
		get_window().mode = Window.MODE_FULLSCREEN

func switch_scene(packed : PackedScene) -> void:
	while _switching:
		await get_tree().process_frame
	_next_scene = packed.instantiate()
	add_child(_next_scene)
	_next_scene.modulate.a = 0
	move_child(_next_scene,0)
	var tween = create_tween().set_parallel()
	tween.tween_property(_current_scene, "modulate:a", 0.0, 0.5)
	tween.tween_property(_next_scene, "modulate:a", 1.0, 0.5)
	_switching = true
	await tween.finished
	_switching = false
	_current_scene.queue_free()
	_current_scene = _next_scene
	_next_scene = null
	
func _handle_settings() -> void:
	var ps : PackedScene = load("res://Scenes/Dialogs/settings_dialog.tscn")
	var dlg : ConfirmationDialog = ps.instantiate()
	Managers.settings.data.snapshot()
	dlg.close_requested.connect(func(): dlg.queue_free(); Managers.settings.data.restore())
	dlg.canceled.connect(func(): dlg.queue_free(); Managers.settings.data.restore())
	dlg.confirmed.connect(_handle_save_settings.bind(dlg))
	dlg.popup_exclusive_centered(self)

func _handle_save_settings(dlg : ConfirmationDialog) -> void:
	Managers.settings.save_settings()
	dlg.queue_free()

func _handle_shutdown() -> void:
	get_tree().quit()
