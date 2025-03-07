@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchUserBlockList

## An ID that identifies the blocked user.
var user_id: String:
	set(val):
		user_id = val
		changed_data["user_id"] = user_id
## The blocked user’s login name.
var user_login: String:
	set(val):
		user_login = val
		changed_data["user_login"] = user_login
## The blocked user’s display name.
var display_name: String:
	set(val):
		display_name = val
		changed_data["display_name"] = display_name

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchUserBlockList:
	var result = TwitchUserBlockList.new()
	if d.has("user_id") && d["user_id"] != null:
		result.user_id = d["user_id"]
	if d.has("user_login") && d["user_login"] != null:
		result.user_login = d["user_login"]
	if d.has("display_name") && d["display_name"] != null:
		result.display_name = d["display_name"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

