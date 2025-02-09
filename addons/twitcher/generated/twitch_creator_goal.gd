@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchCreatorGoal

## An ID that identifies this goal.
var id: String:
	set(val):
		id = val
		changed_data["id"] = id
## An ID that identifies the broadcaster that created the goal.
var broadcaster_id: String:
	set(val):
		broadcaster_id = val
		changed_data["broadcaster_id"] = broadcaster_id
## The broadcaster’s display name.
var broadcaster_name: String:
	set(val):
		broadcaster_name = val
		changed_data["broadcaster_name"] = broadcaster_name
## The broadcaster’s login name.
var broadcaster_login: String:
	set(val):
		broadcaster_login = val
		changed_data["broadcaster_login"] = broadcaster_login
## The type of goal. Possible values are:       * follower — The goal is to increase followers. * subscription — The goal is to increase subscriptions. This type shows the net increase or decrease in tier points associated with the subscriptions. * subscription\_count — The goal is to increase subscriptions. This type shows the net increase or decrease in the number of subscriptions. * new\_subscription — The goal is to increase subscriptions. This type shows only the net increase in tier points associated with the subscriptions (it does not account for users that unsubscribed since the goal started). * new\_subscription\_count — The goal is to increase subscriptions. This type shows only the net increase in the number of subscriptions (it does not account for users that unsubscribed since the goal started).
var type: String:
	set(val):
		type = val
		changed_data["type"] = type
## A description of the goal. Is an empty string if not specified.
var description: String:
	set(val):
		description = val
		changed_data["description"] = description
## The goal’s current value.      The goal’s `type` determines how this value is increased or decreased.       * If `type` is follower, this field is set to the broadcaster's current number of followers. This number increases with new followers and decreases when users unfollow the broadcaster. * If `type` is subscription, this field is increased and decreased by the points value associated with the subscription tier. For example, if a tier-two subscription is worth 2 points, this field is increased or decreased by 2, not 1. * If `type` is subscription\_count, this field is increased by 1 for each new subscription and decreased by 1 for each user that unsubscribes. * If `type` is new\_subscription, this field is increased by the points value associated with the subscription tier. For example, if a tier-two subscription is worth 2 points, this field is increased by 2, not 1. * If `type` is new\_subscription\_count, this field is increased by 1 for each new subscription.
var current_amount: int:
	set(val):
		current_amount = val
		changed_data["current_amount"] = current_amount
## The goal’s target value. For example, if the broadcaster has 200 followers before creating the goal, and their goal is to double that number, this field is set to 400.
var target_amount: int:
	set(val):
		target_amount = val
		changed_data["target_amount"] = target_amount
## The UTC date and time (in RFC3339 format) that the broadcaster created the goal.
var created_at: Variant:
	set(val):
		created_at = val
		changed_data["created_at"] = created_at

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchCreatorGoal:
	var result = TwitchCreatorGoal.new()
	if d.has("id") && d["id"] != null:
		result.id = d["id"]
	if d.has("broadcaster_id") && d["broadcaster_id"] != null:
		result.broadcaster_id = d["broadcaster_id"]
	if d.has("broadcaster_name") && d["broadcaster_name"] != null:
		result.broadcaster_name = d["broadcaster_name"]
	if d.has("broadcaster_login") && d["broadcaster_login"] != null:
		result.broadcaster_login = d["broadcaster_login"]
	if d.has("type") && d["type"] != null:
		result.type = d["type"]
	if d.has("description") && d["description"] != null:
		result.description = d["description"]
	if d.has("current_amount") && d["current_amount"] != null:
		result.current_amount = d["current_amount"]
	if d.has("target_amount") && d["target_amount"] != null:
		result.target_amount = d["target_amount"]
	if d.has("created_at") && d["created_at"] != null:
		result.created_at = d["created_at"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

