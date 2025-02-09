@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchStreamTag

## An ID that identifies this tag.
var tag_id: String:
	set(val):
		tag_id = val
		changed_data["tag_id"] = tag_id
## A Boolean value that determines whether the tag is an automatic tag. An automatic tag is one that Twitch adds to the stream. Broadcasters may not add automatic tags to their channel. The value is **true** if the tag is an automatic tag; otherwise, **false**.
var is_auto: bool:
	set(val):
		is_auto = val
		changed_data["is_auto"] = is_auto
## A dictionary that contains the localized names of the tag. The key is in the form, <locale>-<coutry/region>. For example, en-us. The value is the localized name.
var localization_names: Dictionary:
	set(val):
		localization_names = val
		changed_data["localization_names"] = localization_names
## A dictionary that contains the localized descriptions of the tag. The key is in the form, <locale>-<coutry/region>. For example, en-us. The value is the localized description.
var localization_descriptions: Dictionary:
	set(val):
		localization_descriptions = val
		changed_data["localization_descriptions"] = localization_descriptions

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchStreamTag:
	var result = TwitchStreamTag.new()
	if d.has("tag_id") && d["tag_id"] != null:
		result.tag_id = d["tag_id"]
	if d.has("is_auto") && d["is_auto"] != null:
		result.is_auto = d["is_auto"]
	if d.has("localization_names") && d["localization_names"] != null:
		result.localization_names = d["localization_names"]
	if d.has("localization_descriptions") && d["localization_descriptions"] != null:
		result.localization_descriptions = d["localization_descriptions"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

