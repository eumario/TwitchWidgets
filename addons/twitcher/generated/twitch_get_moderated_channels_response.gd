@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchGetModeratedChannelsResponse

## The list of channels that the user has moderator privileges in.
var data: Array[Data]:
	set(val):
		data = val
		changed_data["data"] = []
		if data != null:
			for value in data:
				changed_data["data"].append(value.to_dict())
## Contains the information used to page through the list of results. The object is empty if there are no more pages left to page through.
var pagination: Pagination:
	set(val):
		pagination = val
		if pagination != null:
			changed_data["pagination"] = pagination.to_dict()

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchGetModeratedChannelsResponse:
	var result = TwitchGetModeratedChannelsResponse.new()
	if d.has("data") && d["data"] != null:
		for value in d["data"]:
			result.data.append(Data.from_json(value))
	if d.has("pagination") && d["pagination"] != null:
		result.pagination = Pagination.from_json(d["pagination"])
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

## 
class Data extends RefCounted:
	## An ID that uniquely identifies the channel this user can moderate.
	var broadcaster_id: String:
		set(val):
			broadcaster_id = val
			changed_data["broadcaster_id"] = broadcaster_id
	## The channel’s login name.
	var broadcaster_login: String:
		set(val):
			broadcaster_login = val
			changed_data["broadcaster_login"] = broadcaster_login
	## The channels’ display name.
	var broadcaster_name: String:
		set(val):
			broadcaster_name = val
			changed_data["broadcaster_name"] = broadcaster_name

	var changed_data: Dictionary = {}

	static func from_json(d: Dictionary) -> Data:
		var result = Data.new()
		if d.has("broadcaster_id") && d["broadcaster_id"] != null:
			result.broadcaster_id = d["broadcaster_id"]
		if d.has("broadcaster_login") && d["broadcaster_login"] != null:
			result.broadcaster_login = d["broadcaster_login"]
		if d.has("broadcaster_name") && d["broadcaster_name"] != null:
			result.broadcaster_name = d["broadcaster_name"]
		return result

	func to_dict() -> Dictionary:
		return changed_data

	func to_json() -> String:
		return JSON.stringify(to_dict())

## Contains the information used to page through the list of results. The object is empty if there are no more pages left to page through.
class Pagination extends RefCounted:
	## The cursor used to get the next page of results. Use the cursor to set the request’s after query parameter.
	var cursor: String:
		set(val):
			cursor = val
			changed_data["cursor"] = cursor

	var changed_data: Dictionary = {}

	static func from_json(d: Dictionary) -> Pagination:
		var result = Pagination.new()
		if d.has("cursor") && d["cursor"] != null:
			result.cursor = d["cursor"]
		return result

	func to_dict() -> Dictionary:
		return changed_data

	func to_json() -> String:
		return JSON.stringify(to_dict())

