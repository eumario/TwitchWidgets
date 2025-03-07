@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchChannelStreamScheduleSegment

## An ID that identifies this broadcast segment.
var id: String:
	set(val):
		id = val
		changed_data["id"] = id
## The UTC date and time (in RFC3339 format) of when the broadcast starts.
var start_time: Variant:
	set(val):
		start_time = val
		changed_data["start_time"] = start_time
## The UTC date and time (in RFC3339 format) of when the broadcast ends.
var end_time: Variant:
	set(val):
		end_time = val
		changed_data["end_time"] = end_time
## The broadcast segment’s title.
var title: String:
	set(val):
		title = val
		changed_data["title"] = title
## Indicates whether the broadcaster canceled this segment of a recurring broadcast. If the broadcaster canceled this segment, this field is set to the same value that’s in the `end_time` field; otherwise, it’s set to **null**.
var canceled_until: String:
	set(val):
		canceled_until = val
		changed_data["canceled_until"] = canceled_until
## The type of content that the broadcaster plans to stream or **null** if not specified.
var category: Category:
	set(val):
		category = val
		if category != null:
			changed_data["category"] = category.to_dict()
## A Boolean value that determines whether the broadcast is part of a recurring series that streams at the same time each week or is a one-time broadcast. Is **true** if the broadcast is part of a recurring series.
var is_recurring: bool:
	set(val):
		is_recurring = val
		changed_data["is_recurring"] = is_recurring

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchChannelStreamScheduleSegment:
	var result = TwitchChannelStreamScheduleSegment.new()
	if d.has("id") && d["id"] != null:
		result.id = d["id"]
	if d.has("start_time") && d["start_time"] != null:
		result.start_time = d["start_time"]
	if d.has("end_time") && d["end_time"] != null:
		result.end_time = d["end_time"]
	if d.has("title") && d["title"] != null:
		result.title = d["title"]
	if d.has("canceled_until") && d["canceled_until"] != null:
		result.canceled_until = d["canceled_until"]
	if d.has("category") && d["category"] != null:
		result.category = Category.from_json(d["category"])
	if d.has("is_recurring") && d["is_recurring"] != null:
		result.is_recurring = d["is_recurring"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

## The type of content that the broadcaster plans to stream or **null** if not specified.
class Category extends RefCounted:
	## An ID that identifies the category that best represents the content that the broadcaster plans to stream. For example, the game’s ID if the broadcaster will play a game or the Just Chatting ID if the broadcaster will host a talk show.
	var id: String:
		set(val):
			id = val
			changed_data["id"] = id
	## The name of the category. For example, the game’s title if the broadcaster will play a game or Just Chatting if the broadcaster will host a talk show.
	var name: String:
		set(val):
			name = val
			changed_data["name"] = name

	var changed_data: Dictionary = {}

	static func from_json(d: Dictionary) -> Category:
		var result = Category.new()
		if d.has("id") && d["id"] != null:
			result.id = d["id"]
		if d.has("name") && d["name"] != null:
			result.name = d["name"]
		return result

	func to_dict() -> Dictionary:
		return changed_data

	func to_json() -> String:
		return JSON.stringify(to_dict())

