@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchGetBitsLeaderboardResponse

## A list of leaderboard leaders. The leaders are returned in rank order by how much they’ve cheered. The array is empty if nobody has cheered bits.
var data: Array[TwitchBitsLeaderboard]:
	set(val):
		data = val
		changed_data["data"] = []
		if data != null:
			for value in data:
				changed_data["data"].append(value.to_dict())
## The reporting window’s start and end dates, in RFC3339 format. The dates are calculated by using the _started\_at_ and _period_ query parameters. If you don’t specify the _started\_at_ query parameter, the fields contain empty strings.
var date_range: DateRange:
	set(val):
		date_range = val
		if date_range != null:
			changed_data["date_range"] = date_range.to_dict()
## The number of ranked users in `data`. This is the value in the _count_ query parameter or the total number of entries on the leaderboard, whichever is less.
var total: int:
	set(val):
		total = val
		changed_data["total"] = total

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchGetBitsLeaderboardResponse:
	var result = TwitchGetBitsLeaderboardResponse.new()
	if d.has("data") && d["data"] != null:
		for value in d["data"]:
			result.data.append(TwitchBitsLeaderboard.from_json(value))
	if d.has("date_range") && d["date_range"] != null:
		result.date_range = DateRange.from_json(d["date_range"])
	if d.has("total") && d["total"] != null:
		result.total = d["total"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

## The reporting window’s start and end dates, in RFC3339 format. The dates are calculated by using the _started\_at_ and _period_ query parameters. If you don’t specify the _started\_at_ query parameter, the fields contain empty strings.
class DateRange extends RefCounted:
	## The reporting window’s start date.
	var started_at: Variant:
		set(val):
			started_at = val
			changed_data["started_at"] = started_at
	## The reporting window’s end date.
	var ended_at: Variant:
		set(val):
			ended_at = val
			changed_data["ended_at"] = ended_at

	var changed_data: Dictionary = {}

	static func from_json(d: Dictionary) -> DateRange:
		var result = DateRange.new()
		if d.has("started_at") && d["started_at"] != null:
			result.started_at = d["started_at"]
		if d.has("ended_at") && d["ended_at"] != null:
			result.ended_at = d["ended_at"]
		return result

	func to_dict() -> Dictionary:
		return changed_data

	func to_json() -> String:
		return JSON.stringify(to_dict())

