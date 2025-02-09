@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchUpdateChatSettingsResponse

## The list of chat settings. The list contains a single object with all the settings.
var data: Array[TwitchChatSettingsUpdated]:
	set(val):
		data = val
		changed_data["data"] = []
		if data != null:
			for value in data:
				changed_data["data"].append(value.to_dict())

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchUpdateChatSettingsResponse:
	var result = TwitchUpdateChatSettingsResponse.new()
	if d.has("data") && d["data"] != null:
		for value in d["data"]:
			result.data.append(TwitchChatSettingsUpdated.from_json(value))
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

