@tool
extends RefCounted

# CLASS GOT AUTOGENERATED DON'T CHANGE MANUALLY. CHANGES CAN BE OVERWRITTEN EASILY.

class_name TwitchCreateEventSubSubscriptionResponse

## A list that contains the single subscription that you created.
var data: Array[TwitchEventSubSubscription]:
	set(val):
		data = val
		changed_data["data"] = []
		if data != null:
			for value in data:
				changed_data["data"].append(value.to_dict())
## The total number of subscriptions you’ve created.
var total: int:
	set(val):
		total = val
		changed_data["total"] = total
## The sum of all of your subscription costs. [Learn More](https://dev.twitch.tv/docs/eventsub/manage-subscriptions/#subscription-limits)
var total_cost: int:
	set(val):
		total_cost = val
		changed_data["total_cost"] = total_cost
## The maximum total cost that you’re allowed to incur for all subscriptions you create.
var max_total_cost: int:
	set(val):
		max_total_cost = val
		changed_data["max_total_cost"] = max_total_cost

var changed_data: Dictionary = {}

static func from_json(d: Dictionary) -> TwitchCreateEventSubSubscriptionResponse:
	var result = TwitchCreateEventSubSubscriptionResponse.new()
	if d.has("data") && d["data"] != null:
		for value in d["data"]:
			result.data.append(TwitchEventSubSubscription.from_json(value))
	if d.has("total") && d["total"] != null:
		result.total = d["total"]
	if d.has("total_cost") && d["total_cost"] != null:
		result.total_cost = d["total_cost"]
	if d.has("max_total_cost") && d["max_total_cost"] != null:
		result.max_total_cost = d["max_total_cost"]
	return result

func to_dict() -> Dictionary:
	return changed_data

func to_json() -> String:
	return JSON.stringify(to_dict())

