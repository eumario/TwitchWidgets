@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchSendWhisperBody

## The whisper message to send. The message must not be empty.      The maximum message lengths are:      * 500 characters if the user you're sending the message to hasn't whispered you before. * 10,000 characters if the user you're sending the message to has whispered you before.    Messages that exceed the maximum length are truncated.
var message: String:
	set(val):
		message = val
		changed_data["message"] = message

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchSendWhisperBody:
	var result = TwitchSendWhisperBody.new()
	if d.has("message") && d["message"] != null:
		result.message = d["message"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

