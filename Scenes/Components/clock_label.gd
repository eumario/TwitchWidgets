@tool
class_name ClockLabel extends Label

@export var show_am_pm : bool = false

const MILITARY_TIME : String = "%02d:%02d"
const MERIDIEM_TIME : String = "%02d:%02d %s"

func _ready() -> void:
	time_format_string_from_system()

func _process(_delta : float) -> void:
	time_format_string_from_system()


func time_format_string_from_system() -> void:
	var dt = Time.get_datetime_dict_from_system()
	if show_am_pm:
		text = MERIDIEM_TIME % [
			dt.hour if dt.hour <= 12 else dt.hour - 12,
			dt.minute,
			"AM" if dt.hour < 12 else "PM"
		]
	else:
		text = MILITARY_TIME % [dt.hour, dt.minute]
