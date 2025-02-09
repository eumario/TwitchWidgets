@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchBanUserResponse

## A list that contains the user you successfully banned or put in a timeout.
var data: Array[Data]:
	set(val):
		data = val
		changed_data["data"] = []
		if data != null:
			for value in data:
				changed_data["data"].append(value.to_dict())

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchBanUserResponse:
	var result = TwitchBanUserResponse.new()
	if d.has("data") && d["data"] != null:
		for value in d["data"]:
			result.data.append(Data.from_json(value))
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

## 
class Data extends RefCounted:
	## The broadcaster whose chat room the user was banned from chatting in.
	var broadcaster_id: String:
		set(val):
			broadcaster_id = val
			changed_data["broadcaster_id"] = broadcaster_id
	## The moderator that banned or put the user in the timeout.
	var moderator_id: String:
		set(val):
			moderator_id = val
			changed_data["moderator_id"] = moderator_id
	## The user that was banned or put in a timeout.
	var user_id: String:
		set(val):
			user_id = val
			changed_data["user_id"] = user_id
	## The UTC date and time (in RFC3339 format) that the ban or timeout was placed.
	var created_at: Variant:
		set(val):
			created_at = val
			changed_data["created_at"] = created_at
	## The UTC date and time (in RFC3339 format) that the timeout will end. Is **null** if the user was banned instead of being put in a timeout.
	var end_time: Variant:
		set(val):
			end_time = val
			changed_data["end_time"] = end_time

	var changed_data: Dictionary = {}

	static func from_json(d: Dictionary) -> Data:
		var result = Data.new()
		if d.has("broadcaster_id") && d["broadcaster_id"] != null:
			result.broadcaster_id = d["broadcaster_id"]
		if d.has("moderator_id") && d["moderator_id"] != null:
			result.moderator_id = d["moderator_id"]
		if d.has("user_id") && d["user_id"] != null:
			result.user_id = d["user_id"]
		if d.has("created_at") && d["created_at"] != null:
			result.created_at = d["created_at"]
		if d.has("end_time") && d["end_time"] != null:
			result.end_time = d["end_time"]
		return result

	func to_dict() -> Dictionary:
		return changed_data

	func to_json() -> String:
		return JSON.stringify(to_dict())

