@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchUserExtensionPanelUpdate

## A Boolean value that determines the extension’s activation state. If **false**, the user has not configured a panel extension.
var active: bool:
	set(val):
		active = val
		changed_data["active"] = active
## An ID that identifies the extension.
var id: String:
	set(val):
		id = val
		changed_data["id"] = id
## The extension’s version.
var version: String:
	set(val):
		version = val
		changed_data["version"] = version

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchUserExtensionPanelUpdate:
	var result = TwitchUserExtensionPanelUpdate.new()
	if d.has("active") && d["active"] != null:
		result.active = d["active"]
	if d.has("id") && d["id"] != null:
		result.id = d["id"]
	if d.has("version") && d["version"] != null:
		result.version = d["version"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

