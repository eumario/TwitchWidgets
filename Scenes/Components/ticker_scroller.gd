@tool
class_name TickerScroller extends Control

@export_category("Message Settings")
@export var messages : Array[String] = []
@export var label_settings : LabelSettings = LabelSettings.new()

@export_category("Speed and Spacing")
@export var speed : float = 30.0
@export var spacing : float = 40.0
@export var separator_text : String = ""

#region Godot Overrides
func _ready() -> void:
	clip_contents = true
	reset_ticker()

func _process(delta : float) -> void:
	for node : Label in get_children():
		node.position.x -= self.speed * delta
	
	for node : Label in get_children():
		if node.position.x < -node.size.x:
			_reset_position(node)

#endregion

#region Private Support API
func _reset_position(lbl : Label) -> void:
	var sx = size.x
	for clbl : Label in get_children():
		if clbl.position.x + clbl.size.x > sx:
			sx = clbl.position.x + clbl.size.x + self.spacing
	
	lbl.position.x = sx

func _make_label(msg : String, px : float) -> float:
	var lbl = Label.new()
	lbl.text = msg
	lbl.label_settings = self.label_settings
	lbl.position = Vector2(px, 0)
	add_child(lbl)
	if size.y < lbl.size.y:
		size = Vector2(size.x, lbl.size.y)
		custom_minimum_size = size
	
	px += lbl.size.x + self.spacing
	return px
#endregion

#region Public API
func add_ticker(msg : String) -> void:
	messages.append(msg)
	reset_ticker()

func remove_ticker(index : int) -> void:
	messages.remove_at(index)
	reset_ticker()

func clear_ticker() -> void:
	messages.clear()
	for node in get_children(): node.queue_free()

func reset_ticker() -> void:
	for node in get_children(): node.queue_free()
	
	var sx = size.x
	for msg in messages:
		sx = _make_label(msg, sx)
		if separator_text != "":
			sx = _make_label(separator_text, sx)
#endregion
