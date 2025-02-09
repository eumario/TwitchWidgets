@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchStream

## An ID that identifies the stream. You can use this ID later to look up the video on demand (VOD).
var id: String:
	set(val):
		id = val
		changed_data["id"] = id
## The ID of the user that’s broadcasting the stream.
var user_id: String:
	set(val):
		user_id = val
		changed_data["user_id"] = user_id
## The user’s login name.
var user_login: String:
	set(val):
		user_login = val
		changed_data["user_login"] = user_login
## The user’s display name.
var user_name: String:
	set(val):
		user_name = val
		changed_data["user_name"] = user_name
## The ID of the category or game being played.
var game_id: String:
	set(val):
		game_id = val
		changed_data["game_id"] = game_id
## The ID of the category or game being played.
var game_name: String:
	set(val):
		game_name = val
		changed_data["game_name"] = game_name
## The type of stream. Possible values are:      * live    If an error occurs, this field is set to an empty string.
var type: String:
	set(val):
		type = val
		changed_data["type"] = type
## The stream’s title. Is an empty string if not set.
var title: String:
	set(val):
		title = val
		changed_data["title"] = title
## The number of users watching the stream.
var viewer_count: int:
	set(val):
		viewer_count = val
		changed_data["viewer_count"] = viewer_count
## The UTC date and time (in RFC3339 format) of when the broadcast began.
var started_at: Variant:
	set(val):
		started_at = val
		changed_data["started_at"] = started_at
## The language that the stream uses. This is an ISO 639-1 two-letter language code or _other_ if the stream uses a language not in the list of [supported stream languages](https://help.twitch.tv/s/article/languages-on-twitch#streamlang).
var language: String:
	set(val):
		language = val
		changed_data["language"] = language
## A URL to an image of a frame from the last 5 minutes of the stream. Replace the width and height placeholders in the URL (`{width}x{height}`) with the size of the image you want, in pixels.
var thumbnail_url: String:
	set(val):
		thumbnail_url = val
		changed_data["thumbnail_url"] = thumbnail_url
## **IMPORTANT** As of February 28, 2023, this field is deprecated and returns only an empty array. If you use this field, please update your code to use the `tags` field.      The list of tags that apply to the stream. The list contains IDs only when the channel is steaming live. For a list of possible tags, see [List of All Tags](https://www.twitch.tv/directory/all/tags). The list doesn’t include Category Tags.
var tag_ids: Array[String]:
	set(val):
		tag_ids = val
		changed_data["tag_ids"] = []
		if tag_ids != null:
			for value in tag_ids:
				changed_data["tag_ids"].append(value)
## The tags applied to the stream.
var tags: Array[String]:
	set(val):
		tags = val
		changed_data["tags"] = []
		if tags != null:
			for value in tags:
				changed_data["tags"].append(value)
## A Boolean value that indicates whether the stream is meant for mature audiences.
var is_mature: bool:
	set(val):
		is_mature = val
		changed_data["is_mature"] = is_mature

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchStream:
	var result = TwitchStream.new()
	if d.has("id") && d["id"] != null:
		result.id = d["id"]
	if d.has("user_id") && d["user_id"] != null:
		result.user_id = d["user_id"]
	if d.has("user_login") && d["user_login"] != null:
		result.user_login = d["user_login"]
	if d.has("user_name") && d["user_name"] != null:
		result.user_name = d["user_name"]
	if d.has("game_id") && d["game_id"] != null:
		result.game_id = d["game_id"]
	if d.has("game_name") && d["game_name"] != null:
		result.game_name = d["game_name"]
	if d.has("type") && d["type"] != null:
		result.type = d["type"]
	if d.has("title") && d["title"] != null:
		result.title = d["title"]
	if d.has("viewer_count") && d["viewer_count"] != null:
		result.viewer_count = d["viewer_count"]
	if d.has("started_at") && d["started_at"] != null:
		result.started_at = d["started_at"]
	if d.has("language") && d["language"] != null:
		result.language = d["language"]
	if d.has("thumbnail_url") && d["thumbnail_url"] != null:
		result.thumbnail_url = d["thumbnail_url"]
	if d.has("tag_ids") && d["tag_ids"] != null:
		for value in d["tag_ids"]:
			result.tag_ids.append(value)
	if d.has("tags") && d["tags"] != null:
		for value in d["tags"]:
			result.tags.append(value)
	if d.has("is_mature") && d["is_mature"] != null:
		result.is_mature = d["is_mature"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

