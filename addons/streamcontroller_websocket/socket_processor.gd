class_name SocketProcessor extends RefCounted

# Packet Format:
# {
#		"event": ["keyDown"|"keyUp"],
#		"identifier": "dev.eumario.WebSocket",
#		"action": "actionCommand",
#		["arguments": "arguments provided here as a string"]
# }

const ButtonEvent = {
	KEY_UP = "keyUp",
	KEY_DOWN = "keyDown"
}

@export var identifier := "dev.eumario.WebSocket"

func _process_packet(data : Dictionary) -> void:
	pass

func _setup() -> void:
	pass

func processs_packet(data : Dictionary) -> void:
	if not _valid_packet(data): return
	_process_packet(data)

func _valid_packet(data : Dictionary) -> bool:
	return data.has("event") and data.has("identifier") and data.has("action") and data.identifier == identifier

func setup() -> void:
	_setup()
