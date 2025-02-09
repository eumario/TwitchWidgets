@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchGetTopGamesResponse

## The list of broadcasts. The broadcasts are sorted by the number of viewers, with the most popular first.
var data: Array[TwitchGame]:
	set(val):
		data = val
		changed_data["data"] = []
		if data != null:
			for value in data:
				changed_data["data"].append(value.to_dict())
## Contains the information used to page through the list of results. The object is empty if there are no more pages left to page through. [Read More](https://dev.twitch.tv/docs/api/guide#pagination)
var pagination: Pagination:
	set(val):
		pagination = val
		if pagination != null:
			changed_data["pagination"] = pagination.to_dict()

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchGetTopGamesResponse:
	var result = TwitchGetTopGamesResponse.new()
	if d.has("data") && d["data"] != null:
		for value in d["data"]:
			result.data.append(TwitchGame.from_json(value))
	if d.has("pagination") && d["pagination"] != null:
		result.pagination = Pagination.from_json(d["pagination"])
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

## Contains the information used to page through the list of results. The object is empty if there are no more pages left to page through. [Read More](https://dev.twitch.tv/docs/api/guide#pagination)
class Pagination extends RefCounted:
	## The cursor used to get the next page of results. Use the cursor to set the request’s _after_ or _before_ query parameter to get the next or previous page of results.
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

