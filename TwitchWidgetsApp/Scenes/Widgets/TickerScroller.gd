@tool
extends Control
class_name GDTickerScroller

#region Public Exports
@export var messages : Array[String] = []
@export var label_settings : LabelSettings = null
@export var speed : float = 30.0
@export var spacing : float = 40
@export var separator_text : String = ""
#endregion

#region Godot Overrides
func _ready() -> void:
	reset_ticker()

func _process(delta) -> void:
	for node in get_children():
		node.position.x -= (speed * delta)
	
	for node in get_children():
		if node.position.x < -node.size.x:
			reset_position(node)
#endregion

#region Public API
func add_ticker(message : String) -> void:
	messages.append(message)
	reset_ticker()

func remove_message(index : int) -> void:
	messages.remove_at(index)
	reset_ticker()

func reset_position(lbl : Label) -> void:
	var sx = size.x
	for x in get_children():
		if x.position.x + x.size.x > sx:
			sx = x.position.x + x.size.x + spacing
	lbl.position = Vector2(sx, 0)

func reset_ticker() -> void:
	for node in get_children():
		node.queue_free()
	
	var sx = size.x
	for msg in messages:
		var lbl = Label.new()
		lbl.text = msg
		lbl.label_settings = label_settings
		lbl.position = Vector2(sx, 0)
		add_child(lbl)
		if size.y < lbl.size.y:
			size.y = lbl.size.y
		sx += lbl.size.x + spacing
		if separator_text != "":
			lbl = Label.new()
			lbl.text = separator_text
			lbl.label_settings = label_settings
			lbl.position = Vector2(sx, 0)
			add_child(lbl)
			sx += lbl.size.x + spacing
#endregion
