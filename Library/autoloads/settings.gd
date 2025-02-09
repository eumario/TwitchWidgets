class_name SettingsManager extends Node

var data : SettingsFile

signal loaded()
signal updated()

func _enter_tree() -> void:
	data = SettingsFile.load_settings()
	loaded.emit()

func save_settings(emitsignal : bool = true) -> void:
	data.save_settings()
	if emitsignal:
		updated.emit()
