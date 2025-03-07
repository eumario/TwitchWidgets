@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchSendChatMessageResponse

## 
var data: Array[Data]:
	set(val):
		data = val
		changed_data["data"] = []
		if data != null:
			for value in data:
				changed_data["data"].append(value.to_dict())

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchSendChatMessageResponse:
	var result = TwitchSendChatMessageResponse.new()
	if d.has("data") && d["data"] != null:
		for value in d["data"]:
			result.data.append(Data.from_json(value))
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

## The reason the message was dropped, if any.
class DropReason extends RefCounted:
	## Code for why the message was dropped.
	var code: String:
		set(val):
			code = val
			changed_data["code"] = code
	## Message for why the message was dropped.
	var message: String:
		set(val):
			message = val
			changed_data["message"] = message

	var changed_data: Dictionary = {}

	static func from_json(d: Dictionary) -> DropReason:
		var result = DropReason.new()
		if d.has("code") && d["code"] != null:
			result.code = d["code"]
		if d.has("message") && d["message"] != null:
			result.message = d["message"]
		return result

	func to_dict() -> Dictionary:
		return changed_data

	func to_json() -> String:
		return JSON.stringify(to_dict())

## 
class Data extends RefCounted:
	## The message id for the message that was sent.
	var message_id: String:
		set(val):
			message_id = val
			changed_data["message_id"] = message_id
	## If the message passed all checks and was sent.
	var is_sent: bool:
		set(val):
			is_sent = val
			changed_data["is_sent"] = is_sent
	## The reason the message was dropped, if any.
	var drop_reason: DropReason:
		set(val):
			drop_reason = val
			if drop_reason != null:
				changed_data["drop_reason"] = drop_reason.to_dict()

	var changed_data: Dictionary = {}

	static func from_json(d: Dictionary) -> Data:
		var result = Data.new()
		if d.has("message_id") && d["message_id"] != null:
			result.message_id = d["message_id"]
		if d.has("is_sent") && d["is_sent"] != null:
			result.is_sent = d["is_sent"]
		if d.has("drop_reason") && d["drop_reason"] != null:
			result.drop_reason = DropReason.from_json(d["drop_reason"])
		return result

	func to_dict() -> Dictionary:
		return changed_data

	func to_json() -> String:
		return JSON.stringify(to_dict())

