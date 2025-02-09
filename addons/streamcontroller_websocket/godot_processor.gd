class_name GodotProcessor extends SocketProcessor

const Actions = {
	EMIT_SIGNAL = "godot.emitsignal",
	SWITCH_SCENE = "godot.switchscene",
	RELOAD_SCENE = "godot.reloadscene"
}

signal on_key_up(args)
signal on_key_down(args)

func _setup() -> void:
	identifier = "games.boyne.godot"

func _process_packet(data : Dictionary) -> void:
	match data.action:
		Actions.EMIT_SIGNAL:
			if not data.has("arguments"):
				return
			
			match data.event:
				ButtonEvent.KEY_UP:
					on_key_up.emit(data.arguments)
				ButtonEvent.KEY_DOWN:
					on_key_down.emit(data.arguments)
		Actions.SWITCH_SCENE:
			if not data.has("arguments"):
				return
			Engine.get_main_loop().change_scene_to_file(data.arguments)
		Actions.RELOAD_SCENE:
			Engine.get_main_loop().reload_current_scene()
