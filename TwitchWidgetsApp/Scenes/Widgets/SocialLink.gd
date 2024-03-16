@tool
extends PanelContainer

@onready var icon : FontAwesome = %SocialIcon
@onready var text : Label = %Text

@export_category("Icon Settings")
@export var icon_size : int :
	set(value):
		icon_size = value
		if (icon != null):
			icon.icon_size = value
	get:
		return icon_size

@export var icon_color : Color :
	set(value):
		icon_color = value
		if (icon != null):
			icon.self_modulate = value
	get:
		return icon_color

@export_enum("solid","regular","brands") var icon_class : String :
	set(value):
		icon_class = value
		if (icon != null):
			icon.icon_type = value
	get:
		return icon_class

@export var icon_name : String :
	set(value):
		icon_name = value
		if (icon != null):
			icon.icon_name = value
	get:
		return icon_name

@export_category("Text Settings")
var font_settings : LabelSettings

@export var display_text : String :
	set(value):
		display_text = value
		if (text != null):
			text.text = value
	get:
		return display_text

@export var text_font : FontFile :
	set(value):
		text_font = value
		if (font_settings != null):
			font_settings.font = value
	get:
		return text_font

@export var text_size : int :
	set(value):
		text_size = value
		if (font_settings != null):
			font_settings.font_size = value
	get:
		return text_size

@export var text_color : Color :
	set(value):
		text_color = value
		if (font_settings != null):
			font_settings.font_color = value
	get:
		return text_color

# Called when the node enters the scene tree for the first time.
func _ready():
	font_settings = text.label_settings.duplicate(true)
	text.label_settings = font_settings
	icon_size = icon_size
	icon_color = icon_color
	icon_class = icon_class
	icon_name = icon_name
	
	display_text = display_text
	text_font = text_font
	text_size = text_size
	text_color = text_color
	pass

