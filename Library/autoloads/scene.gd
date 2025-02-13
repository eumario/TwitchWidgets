class_name SceneManager extends Node

#region Private Variables
var _scenes : Array[SceneCollection] = []
#endregion

#region Properties
var current_collection : SceneCollection = null
var main_window : MainScreen = null
var current_scene_name : String = ""
#endregion

#region Godot Overrides
func _ready() -> void:
	Managers.settings.updated.connect(func():
		set_current_collection(Managers.settings.data.collection_name)
		Logger.info("Scene Collection set to %s" % (current_collection.collection_name if current_collection else "NONE"))
	)
	
	Managers.init_finished.connect(func():
		Logger.info("Finalizing SceneManager initalization...")
		if Managers.settings.data.collection_name != "":
			set_current_collection(Managers.settings.data.collection_name)
			Logger.info("INIT: Scene Collection set to %s" % current_collection.collection_name)
	)
	
	Managers.obs.connection_ready.connect(_handle_connect)
	Managers.obs.scene_changed.connect(_handle_scene_changed)
#endregion

#region Public API
func get_scene_collections() -> Array:
	return _scenes

func add_scene_collection(coll : SceneCollection) -> void:
	_scenes.append(coll)

func set_current_collection(scene_collection_name : String) -> void:
	current_collection = Utils.first(_scenes, func(x : SceneCollection): return x.collection_name == scene_collection_name)

func scene_changed(scene_collection_name : String) -> void:
	if current_collection == null: return
	if main_window == null: return
	
	if current_collection.scenes.has(scene_collection_name):
		Logger.info("Switching to scene '%s'" % scene_collection_name)
		main_window.switch_scene(current_collection.scenes[scene_collection_name])
		current_scene_name = scene_collection_name
	else:
		Logger.error("No such Scene in scene collection '%s'" % current_collection.collection_name)


#endregion

#region Signal Handlers
func _handle_connect() -> void:
	var res = await Managers.obs.get_current_program_scene()
	if current_scene_name != res:
		scene_changed(res)

func _handle_scene_changed(scene_name : String) -> void:
	scene_changed(scene_name)
#endregion
