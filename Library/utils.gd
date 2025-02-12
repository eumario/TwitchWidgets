@tool
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

static func lazy_count(i : int) -> String:
	if i > 1_000_000:
		return "%0.2dm" % i
	if i > 1_000:
		return "%0.2dk" % i
	if i > 0:
		return "%d" % i
	return ""

static func random_lazy_count() -> String:
	var i = randi_range(0,5_000_000)
	return lazy_count(i)

static func lazy_time(secs : float) -> String:
	if secs < 60:
		return "%ds" % secs
	elif secs < 3600:
		return "%dm" % int(secs / 60)
	elif secs < 86400:
		return "%dh" % int(secs / 3600)
	else:
		return "%dd" % int(secs / 86400)

static func first(array : Array, lambda : Callable) -> Variant:
	var find = array.filter(lambda)
	if find.size() > 0: return find[0]
	return null
