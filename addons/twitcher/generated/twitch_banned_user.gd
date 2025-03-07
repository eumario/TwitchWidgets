@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchBannedUser

## The ID of the banned user.
var user_id: String:
	set(val):
		user_id = val
		changed_data["user_id"] = user_id
## The banned user’s login name.
var user_login: String:
	set(val):
		user_login = val
		changed_data["user_login"] = user_login
## The banned user’s display name.
var user_name: String:
	set(val):
		user_name = val
		changed_data["user_name"] = user_name
## The UTC date and time (in RFC3339 format) of when the timeout expires, or an empty string if the user is permanently banned.
var expires_at: Variant:
	set(val):
		expires_at = val
		changed_data["expires_at"] = expires_at
## The UTC date and time (in RFC3339 format) of when the user was banned.
var created_at: Variant:
	set(val):
		created_at = val
		changed_data["created_at"] = created_at
## The reason the user was banned or put in a timeout if the moderator provided one.
var reason: String:
	set(val):
		reason = val
		changed_data["reason"] = reason
## The ID of the moderator that banned the user or put them in a timeout.
var moderator_id: String:
	set(val):
		moderator_id = val
		changed_data["moderator_id"] = moderator_id
## The moderator’s login name.
var moderator_login: String:
	set(val):
		moderator_login = val
		changed_data["moderator_login"] = moderator_login
## The moderator’s display name.
var moderator_name: String:
	set(val):
		moderator_name = val
		changed_data["moderator_name"] = moderator_name

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchBannedUser:
	var result = TwitchBannedUser.new()
	if d.has("user_id") && d["user_id"] != null:
		result.user_id = d["user_id"]
	if d.has("user_login") && d["user_login"] != null:
		result.user_login = d["user_login"]
	if d.has("user_name") && d["user_name"] != null:
		result.user_name = d["user_name"]
	if d.has("expires_at") && d["expires_at"] != null:
		result.expires_at = d["expires_at"]
	if d.has("created_at") && d["created_at"] != null:
		result.created_at = d["created_at"]
	if d.has("reason") && d["reason"] != null:
		result.reason = d["reason"]
	if d.has("moderator_id") && d["moderator_id"] != null:
		result.moderator_id = d["moderator_id"]
	if d.has("moderator_login") && d["moderator_login"] != null:
		result.moderator_login = d["moderator_login"]
	if d.has("moderator_name") && d["moderator_name"] != null:
		result.moderator_name = d["moderator_name"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

