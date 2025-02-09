class_name Utils extends RefCounted

static func format_time(secs : float) -> String:
	var minutes : int = floori(secs / 60)
	secs -= floori(minutes * 60)
	var hours : int = floori(minutes / 60)
	minutes -= hours / 60
	if hours > 0:
		return "%02d:%02d:%02d" % [hours, minutes, secs]
	else:
		return "%02d:%02d" % [minutes, secs]
