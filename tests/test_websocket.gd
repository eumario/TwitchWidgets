extends Control

const WEBSOCKET_URL = "ws://127.0.0.1:%s/ws"

const ButtonEvent = {
	KEY_UP = "keyUp",
	KEY_DOWN = "keyDown"
}

signal on_key_up(action : String, arguments : String)
signal on_key_down(action : String, arguments : String)

var _socket := WebSocketPeer.new()

func _ready() -> void:
	var url := WEBSOCKET_URL % 8765
	_socket.connect_to_url(url)
	%Output.push_bold()
	%Output.push_color(Color.CYAN)
	%Output.add_text("Connecting to %s" % url)
	%Output.pop_all()
	%Output.newline()
	

func _process(_delta) -> void:
	_socket.poll()
	
	var state := _socket.get_ready_state()
	
	match state:
		WebSocketPeer.STATE_OPEN:
			while _socket.get_available_packet_count():
				var data = JSON.parse_string(_socket.get_packet().get_string_from_utf8())
				
				if !(data.event == ButtonEvent.KEY_DOWN || data.event == ButtonEvent.KEY_UP):
					return
				
				if data != null and data.has("action") and data.has("arguments"):
					match data.action:
						ButtonEvent.KEY_UP:
							on_key_up.emit(data.action, data.arguments)
						ButtonEvent.KEY_DOWN:
							on_key_down.emit(data.action, data.arguments)
				#var data = _socket.get_packet().get_string_from_utf8()
				%Output.push_color(Color.GREEN)
				%Output.add_text(JSON.stringify(data))
				%Output.pop_all()
				%Output.newline()
		WebSocketPeer.STATE_CLOSING:
			%Output.push_color(Color.YELLOW)
			%Output.add_text("Closing Connection...")
			%Output.pop_all()
			%Output.newline()
		WebSocketPeer.STATE_CLOSED:
			%Output.push_color(Color.RED)
			%Output.add_text("Connection Closed.")
			%Output.pop_all()
			%Output.newline()
