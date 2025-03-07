@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchClip

## An ID that uniquely identifies the clip.
var id: String:
	set(val):
		id = val
		changed_data["id"] = id
## A URL to the clip.
var url: String:
	set(val):
		url = val
		changed_data["url"] = url
## A URL that you can use in an iframe to embed the clip (see [Embedding Video and Clips](https://dev.twitch.tv/docs/embed/video-and-clips/)).
var embed_url: String:
	set(val):
		embed_url = val
		changed_data["embed_url"] = embed_url
## An ID that identifies the broadcaster that the video was clipped from.
var broadcaster_id: String:
	set(val):
		broadcaster_id = val
		changed_data["broadcaster_id"] = broadcaster_id
## The broadcaster’s display name.
var broadcaster_name: String:
	set(val):
		broadcaster_name = val
		changed_data["broadcaster_name"] = broadcaster_name
## An ID that identifies the user that created the clip.
var creator_id: String:
	set(val):
		creator_id = val
		changed_data["creator_id"] = creator_id
## The user’s display name.
var creator_name: String:
	set(val):
		creator_name = val
		changed_data["creator_name"] = creator_name
## An ID that identifies the video that the clip came from. This field contains an empty string if the video is not available.
var video_id: String:
	set(val):
		video_id = val
		changed_data["video_id"] = video_id
## The ID of the game that was being played when the clip was created.
var game_id: String:
	set(val):
		game_id = val
		changed_data["game_id"] = game_id
## The ISO 639-1 two-letter language code that the broadcaster broadcasts in. For example, _en_ for English. The value is _other_ if the broadcaster uses a language that Twitch doesn’t support.
var language: String:
	set(val):
		language = val
		changed_data["language"] = language
## The title of the clip.
var title: String:
	set(val):
		title = val
		changed_data["title"] = title
## The number of times the clip has been viewed.
var view_count: int:
	set(val):
		view_count = val
		changed_data["view_count"] = view_count
## The date and time of when the clip was created. The date and time is in RFC3339 format.
var created_at: Variant:
	set(val):
		created_at = val
		changed_data["created_at"] = created_at
## A URL to a thumbnail image of the clip.
var thumbnail_url: String:
	set(val):
		thumbnail_url = val
		changed_data["thumbnail_url"] = thumbnail_url
## The length of the clip, in seconds. Precision is 0.1.
var duration: Variant:
	set(val):
		duration = val
		changed_data["duration"] = duration
## The zero-based offset, in seconds, to where the clip starts in the video (VOD). Is **null** if the video is not available or hasn’t been created yet from the live stream (see `video_id`).      Note that there’s a delay between when a clip is created during a broadcast and when the offset is set. During the delay period, `vod_offset` is **null**. The delay is indeterminant but is typically minutes long.
var vod_offset: int:
	set(val):
		vod_offset = val
		changed_data["vod_offset"] = vod_offset
## A Boolean value that indicates if the clip is featured or not.
var is_featured: bool:
	set(val):
		is_featured = val
		changed_data["is_featured"] = is_featured

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchClip:
	var result = TwitchClip.new()
	if d.has("id") && d["id"] != null:
		result.id = d["id"]
	if d.has("url") && d["url"] != null:
		result.url = d["url"]
	if d.has("embed_url") && d["embed_url"] != null:
		result.embed_url = d["embed_url"]
	if d.has("broadcaster_id") && d["broadcaster_id"] != null:
		result.broadcaster_id = d["broadcaster_id"]
	if d.has("broadcaster_name") && d["broadcaster_name"] != null:
		result.broadcaster_name = d["broadcaster_name"]
	if d.has("creator_id") && d["creator_id"] != null:
		result.creator_id = d["creator_id"]
	if d.has("creator_name") && d["creator_name"] != null:
		result.creator_name = d["creator_name"]
	if d.has("video_id") && d["video_id"] != null:
		result.video_id = d["video_id"]
	if d.has("game_id") && d["game_id"] != null:
		result.game_id = d["game_id"]
	if d.has("language") && d["language"] != null:
		result.language = d["language"]
	if d.has("title") && d["title"] != null:
		result.title = d["title"]
	if d.has("view_count") && d["view_count"] != null:
		result.view_count = d["view_count"]
	if d.has("created_at") && d["created_at"] != null:
		result.created_at = d["created_at"]
	if d.has("thumbnail_url") && d["thumbnail_url"] != null:
		result.thumbnail_url = d["thumbnail_url"]
	if d.has("duration") && d["duration"] != null:
		result.duration = d["duration"]
	if d.has("vod_offset") && d["vod_offset"] != null:
		result.vod_offset = d["vod_offset"]
	if d.has("is_featured") && d["is_featured"] != null:
		result.is_featured = d["is_featured"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

