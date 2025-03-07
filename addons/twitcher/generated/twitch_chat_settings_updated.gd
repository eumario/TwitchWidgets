@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchChatSettingsUpdated

## The ID of the broadcaster specified in the request.
var broadcaster_id: String:
	set(val):
		broadcaster_id = val
		changed_data["broadcaster_id"] = broadcaster_id
## A Boolean value that determines whether chat messages must contain only emotes. Is **true** if chat messages may contain only emotes; otherwise, **false**.
var emote_mode: bool:
	set(val):
		emote_mode = val
		changed_data["emote_mode"] = emote_mode
## A Boolean value that determines whether the broadcaster restricts the chat room to followers only.      Is **true** if the broadcaster restricts the chat room to followers only; otherwise, **false**.      See the `follower_mode_duration` field for how long users must follow the broadcaster before being able to participate in the chat room.
var follower_mode: bool:
	set(val):
		follower_mode = val
		changed_data["follower_mode"] = follower_mode
## The length of time, in minutes, that users must follow the broadcaster before being able to participate in the chat room. Is **null** if `follower_mode` is **false**.
var follower_mode_duration: int:
	set(val):
		follower_mode_duration = val
		changed_data["follower_mode_duration"] = follower_mode_duration
## The moderator’s ID. The response includes this field only if the request specifies a user access token that includes the **moderator:read:chat\_settings** scope.
var moderator_id: String:
	set(val):
		moderator_id = val
		changed_data["moderator_id"] = moderator_id
## A Boolean value that determines whether the broadcaster adds a short delay before chat messages appear in the chat room. This gives chat moderators and bots a chance to remove them before viewers can see the message. See the `non_moderator_chat_delay_duration` field for the length of the delay. Is **true** if the broadcaster applies a delay; otherwise, **false**.
var non_moderator_chat_delay: bool:
	set(val):
		non_moderator_chat_delay = val
		changed_data["non_moderator_chat_delay"] = non_moderator_chat_delay
## The amount of time, in seconds, that messages are delayed before appearing in chat. Is **null** if `non_moderator_chat_delay` is **false**.
var non_moderator_chat_delay_duration: int:
	set(val):
		non_moderator_chat_delay_duration = val
		changed_data["non_moderator_chat_delay_duration"] = non_moderator_chat_delay_duration
## A Boolean value that determines whether the broadcaster limits how often users in the chat room are allowed to send messages.      Is **true** if the broadcaster applies a delay; otherwise, **false**.      See the `slow_mode_wait_time` field for the delay.
var slow_mode: bool:
	set(val):
		slow_mode = val
		changed_data["slow_mode"] = slow_mode
## The amount of time, in seconds, that users must wait between sending messages.      Is **null** if slow\_mode is **false**.
var slow_mode_wait_time: int:
	set(val):
		slow_mode_wait_time = val
		changed_data["slow_mode_wait_time"] = slow_mode_wait_time
## A Boolean value that determines whether only users that subscribe to the broadcaster’s channel may talk in the chat room.      Is **true** if the broadcaster restricts the chat room to subscribers only; otherwise, **false**.
var subscriber_mode: bool:
	set(val):
		subscriber_mode = val
		changed_data["subscriber_mode"] = subscriber_mode
## A Boolean value that determines whether the broadcaster requires users to post only unique messages in the chat room.      Is **true** if the broadcaster requires unique messages only; otherwise, **false**.
var unique_chat_mode: bool:
	set(val):
		unique_chat_mode = val
		changed_data["unique_chat_mode"] = unique_chat_mode

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchChatSettingsUpdated:
	var result = TwitchChatSettingsUpdated.new()
	if d.has("broadcaster_id") && d["broadcaster_id"] != null:
		result.broadcaster_id = d["broadcaster_id"]
	if d.has("emote_mode") && d["emote_mode"] != null:
		result.emote_mode = d["emote_mode"]
	if d.has("follower_mode") && d["follower_mode"] != null:
		result.follower_mode = d["follower_mode"]
	if d.has("follower_mode_duration") && d["follower_mode_duration"] != null:
		result.follower_mode_duration = d["follower_mode_duration"]
	if d.has("moderator_id") && d["moderator_id"] != null:
		result.moderator_id = d["moderator_id"]
	if d.has("non_moderator_chat_delay") && d["non_moderator_chat_delay"] != null:
		result.non_moderator_chat_delay = d["non_moderator_chat_delay"]
	if d.has("non_moderator_chat_delay_duration") && d["non_moderator_chat_delay_duration"] != null:
		result.non_moderator_chat_delay_duration = d["non_moderator_chat_delay_duration"]
	if d.has("slow_mode") && d["slow_mode"] != null:
		result.slow_mode = d["slow_mode"]
	if d.has("slow_mode_wait_time") && d["slow_mode_wait_time"] != null:
		result.slow_mode_wait_time = d["slow_mode_wait_time"]
	if d.has("subscriber_mode") && d["subscriber_mode"] != null:
		result.subscriber_mode = d["subscriber_mode"]
	if d.has("unique_chat_mode") && d["unique_chat_mode"] != null:
		result.unique_chat_mode = d["unique_chat_mode"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

