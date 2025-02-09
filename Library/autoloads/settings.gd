class_name SettingsManager extends Node

var data : SettingsFile

func _enter_tree() -> void:
	data = SettingsFile.load_settings()
