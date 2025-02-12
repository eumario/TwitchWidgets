class_name SceneManager extends Node

#region Private Variables
var _scenes : Array[SceneCollection] = []
#endregion

#region Properties
var current_collection : SceneCollection = null
var main_window : MainScreen = null
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
#endregion

#region Public API
func get_scene_collections() -> Array:
	return _scenes

func add_scene_collection(coll : SceneCollection) -> void:
	_scenes.append(coll)

func set_current_collection(name : String) -> void:
	current_collection = Utils.first(_scenes, func(x : SceneCollection): return x.collection_name == name)

func scene_changed(name : String) -> void:
	if current_collection == null: return
	if main_window == null: return
	
	if current_collection.scenes.has(name):
		Logger.info("Switching to scene '%s'" % name)
		main_window.switch_scene(current_collection.scenes[name])
	else:
		Logger.error("No such Scene in scene collection '%s'" % current_collection.collection_name)
#endregion
