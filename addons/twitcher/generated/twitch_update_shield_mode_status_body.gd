@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchUpdateShieldModeStatusBody

## A Boolean value that determines whether to activate Shield Mode. Set to **true** to activate Shield Mode; otherwise, **false** to deactivate Shield Mode.
var is_active: bool:
	set(val):
		is_active = val
		changed_data["is_active"] = is_active

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchUpdateShieldModeStatusBody:
	var result = TwitchUpdateShieldModeStatusBody.new()
	if d.has("is_active") && d["is_active"] != null:
		result.is_active = d["is_active"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

