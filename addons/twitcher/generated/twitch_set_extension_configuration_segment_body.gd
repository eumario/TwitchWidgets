@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchSetExtensionConfigurationSegmentBody

## The ID of the extension to update.
var extension_id: String:
	set(val):
		extension_id = val
		changed_data["extension_id"] = extension_id
## The configuration segment to update. Possible case-sensitive values are:      * broadcaster * developer * global
var segment: String:
	set(val):
		segment = val
		changed_data["segment"] = segment
## The ID of the broadcaster that installed the extension. Include this field only if the `segment` is set to developer or broadcaster.
var broadcaster_id: String:
	set(val):
		broadcaster_id = val
		changed_data["broadcaster_id"] = broadcaster_id
## The contents of the segment. This string may be a plain-text string or a string-encoded JSON object.
var content: String:
	set(val):
		content = val
		changed_data["content"] = content
## The version number that identifies this definition of the segment’s data. If not specified, the latest definition is updated.
var version: String:
	set(val):
		version = val
		changed_data["version"] = version

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchSetExtensionConfigurationSegmentBody:
	var result = TwitchSetExtensionConfigurationSegmentBody.new()
	if d.has("extension_id") && d["extension_id"] != null:
		result.extension_id = d["extension_id"]
	if d.has("segment") && d["segment"] != null:
		result.segment = d["segment"]
	if d.has("broadcaster_id") && d["broadcaster_id"] != null:
		result.broadcaster_id = d["broadcaster_id"]
	if d.has("content") && d["content"] != null:
		result.content = d["content"]
	if d.has("version") && d["version"] != null:
		result.version = d["version"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

