class_name SCWebsocketClient extends Node

var socket : WebSocketPeer
var processor : SocketProcessor

var last_state : WebSocketPeer.State = WebSocketPeer.STATE_CLOSED

signal connecting()
signal connected()
signal closing()
signal closed()
signal recv_packet(data : Dictionary)

func _ready() -> void:
	assert(processor, "You need to provide a Processor for this client.")
	processor.setup()

func connect_to_url(url : String = "ws://localhost:8765") -> Error:
	if socket:
		if socket.get_ready_state() == WebSocketPeer.STATE_OPEN:
			socket.close()
	
	socket = WebSocketPeer.new()
	return socket.connect_to_url(url)

func _process(delta: float) -> void:
	socket.poll()
	
	var state := socket.get_ready_state()
	
	if state == last_state and state != WebSocketPeer.STATE_OPEN:
		return
	
	if last_state == WebSocketPeer.STATE_CONNECTING and state == WebSocketPeer.STATE_OPEN:
		connected.emit()
	
	last_state = state
	match state:
		WebSocketPeer.STATE_OPEN:
			while socket.get_available_packet_count():
				var data = JSON.parse_string(socket.get_packet().get_string_from_utf8())
				
				processor.processs_packet(data)
		WebSocketPeer.STATE_CONNECTING:
			connecting.emit()
		WebSocketPeer.STATE_CLOSING:
			closing.emit()
		WebSocketPeer.STATE_CLOSED:
			closed.emit()
