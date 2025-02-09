@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchCreatePollResponse

## A list that contains the single poll that you created.
var data: Array[TwitchPoll]:
	set(val):
		data = val
		changed_data["data"] = []
		if data != null:
			for value in data:
				changed_data["data"].append(value.to_dict())

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchCreatePollResponse:
	var result = TwitchCreatePollResponse.new()
	if d.has("data") && d["data"] != null:
		for value in d["data"]:
			result.data.append(TwitchPoll.from_json(value))
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

