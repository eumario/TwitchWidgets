class_name TwitchManager extends Node

signal channel_update(data : Dictionary)
signal follow(data : Dictionary)
signal ad_break(data : Dictionary)
signal cheer(data : Dictionary)
signal raid(data : Dictionary)
signal ban(data : Dictionary)
signal chat(data : Dictionary)
signal chat_deleted(data : Dictionary)
signal sub(data : Dictionary)
signal gifted_sub(data : Dictionary)
signal sub_message(data : Dictionary)
signal poll(state : State, data : Dictionary)
signal prediction(state : State, data : Dictionary)
signal goal(state : State, data : Dictionary)
signal shoutout(data : Dictionary)
signal received_shoutout(data : Dictionary)
signal custom_reward(data : Dictionary)

enum State {
	BEGIN,
	PROGRESS,
	LOCK,
	END
}

var twitch_service : TwitchService
var broadcaster : TwitchUser

var EVENTSUB_EVENTS : Array[TwitchEventsubDefinition] = [
	TwitchEventsubDefinition.CHANNEL_UPDATE,
	TwitchEventsubDefinition.CHANNEL_FOLLOW,
	TwitchEventsubDefinition.CHANNEL_AD_BREAK_BEGIN,
	TwitchEventsubDefinition.CHANNEL_CHEER,
	TwitchEventsubDefinition.CHANNEL_RAID,
	TwitchEventsubDefinition.CHANNEL_CHAT_MESSAGE,
	TwitchEventsubDefinition.CHANNEL_CHAT_MESSAGE_DELETE,
	TwitchEventsubDefinition.CHANNEL_SUBSCRIBE,
	TwitchEventsubDefinition.CHANNEL_SUBSCRIPTION_GIFT,
	TwitchEventsubDefinition.CHANNEL_SUBSCRIPTION_MESSAGE,
	TwitchEventsubDefinition.CHANNEL_POLL_BEGIN,
	TwitchEventsubDefinition.CHANNEL_POLL_PROGRESS,
	TwitchEventsubDefinition.CHANNEL_POLL_END,
	TwitchEventsubDefinition.CHANNEL_PREDICTION_BEGIN,
	TwitchEventsubDefinition.CHANNEL_PREDICTION_PROGRESS,
	TwitchEventsubDefinition.CHANNEL_PREDICTION_LOCK,
	TwitchEventsubDefinition.CHANNEL_PREDICTION_END,
	TwitchEventsubDefinition.CHANNEL_GOAL_BEGIN,
	TwitchEventsubDefinition.CHANNEL_GOAL_PROGRESS,
	TwitchEventsubDefinition.CHANNEL_GOAL_END,
	TwitchEventsubDefinition.CHANNEL_SHOUTOUT_CREATE,
	TwitchEventsubDefinition.CHANNEL_SHOUTOUT_RECEIVE,
]

func _ready() -> void:
	var settings = Managers.settings.data
	if settings.client_id == "" and settings.client_secret == "": return
	if settings.auto_connect_twitch:
		setup_auth_info(settings.client_id, settings.client_secret)

func _wait_for_ready() -> void:
	await twitch_service.eventsub.wait_for_session_established()
	var info : TwitchGetUsersResponse = await twitch_service.api.get_users([], [])
	broadcaster = info.data[0]

func _enable_events() -> void:
	twitch_service.eventsub.event.connect(_handle_eventsub)

func _handle_eventsub(type : String, data : Dictionary) -> void:
	if type == TwitchEventsubDefinition.CHANNEL_UPDATE.get_readable_name(): channel_update.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_FOLLOW.get_readable_name(): follow.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_AD_BREAK_BEGIN.get_readable_name(): ad_break.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_CHEER.get_readable_name(): cheer.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_RAID.get_readable_name(): raid.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_BAN.get_readable_name(): ban.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_CHAT_MESSAGE.get_readable_name(): chat.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_CHAT_MESSAGE_DELETE.get_readable_name(): chat_deleted.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_SUBSCRIBE.get_readable_name(): sub.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_SUBSCRIPTION_GIFT.get_readable_name(): gifted_sub.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_SUBSCRIPTION_MESSAGE.get_readable_name(): sub_message.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_POLL_BEGIN.get_readable_name(): poll.emit(State.BEGIN, data)
	if type == TwitchEventsubDefinition.CHANNEL_POLL_PROGRESS.get_readable_name(): poll.emit(State.PROGRESS, data)
	if type == TwitchEventsubDefinition.CHANNEL_POLL_END.get_readable_name(): poll.emit(State.END, data)
	if type == TwitchEventsubDefinition.CHANNEL_PREDICTION_BEGIN.get_readable_name(): prediction.emit(State.BEGIN, data)
	if type == TwitchEventsubDefinition.CHANNEL_PREDICTION_PROGRESS.get_readable_name(): prediction.emit(State.PROGRESS, data)
	if type == TwitchEventsubDefinition.CHANNEL_PREDICTION_LOCK.get_readable_name(): prediction.emit(State.LOCK, data)
	if type == TwitchEventsubDefinition.CHANNEL_PREDICTION_END.get_readable_name(): prediction.emit(State.END,data)
	if type == TwitchEventsubDefinition.CHANNEL_SHOUTOUT_CREATE.get_readable_name(): shoutout.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_SHOUTOUT_RECEIVE.get_readable_name(): received_shoutout.emit(data)
	if type == TwitchEventsubDefinition.CHANNEL_GOAL_BEGIN.get_readable_name(): goal.emit(State.BEGIN, data)
	if type == TwitchEventsubDefinition.CHANNEL_GOAL_PROGRESS.get_readable_name(): goal.emit(State.PROGRESS, data)
	if type == TwitchEventsubDefinition.CHANNEL_GOAL_END.get_readable_name(): goal.emit(State.END, data)
	if type == TwitchEventsubDefinition.CHANNEL_CHANNEL_POINTS_CUSTOM_REWARD_REDEMPTION_ADD.get_readable_name(): custom_reward.emit(data)

func send_chat_message(message : String) -> void:
	var body := TwitchSendChatMessageBody.new()
	body.broadcaster_id = broadcaster.id
	body.sender_id = broadcaster.id
	body.message = message
	
	Logger.debug("Sending Message: %s> %s" % [broadcaster.login, message])
	var _res : TwitchSendChatMessageResponse = await twitch_service.api.send_chat_message(body)

func send_reply_message(id : String, message : String) -> void:
	var body := TwitchSendChatMessageBody.new()
	body.broadcaster_id = broadcaster.id
	body.sender_id = broadcaster.id
	body.message = message
	body.reply_parent_message_id = id
	
	Logger.debug("Sending Reply Message: %s:(%s)> %s" % [broadcaster.id, id, message])
	var _res : TwitchSendChatMessageResponse = await twitch_service.api.send_chat_message(body)

func setup_auth_info(client_id : String, client_secret : String) -> void:
	var oauth : OAuthSetting = load("res://addons/twitcher/default_oauth_setting.tres").duplicate()
	
	oauth.client_id = client_id
	oauth.client_secret = client_secret
	
	_setup_nodes(oauth)
	
	await twitch_service.setup()
	
	var res = await twitch_service.api.get_users([],[])
	broadcaster = res.data[0]
	_setup_eventsub()
	_get_custom_rewards()
	_enable_events()

func _setup_nodes(oauth : OAuthSetting) -> void:
	twitch_service = TwitchService.new()
	twitch_service.name = "TwitchService"
	twitch_service.oauth_setting = oauth
	twitch_service.scopes = load("res://Resources/twitch_scopes.tres")
	twitch_service.token = load("res://Resources/twitch_token.tres")
	var api := TwitchAPI.new()
	api.name = "Api"
	twitch_service.add_child(api)
	var eventsub := TwitchEventsub.new()
	eventsub.name = "EventSub"
	eventsub.api = api
	twitch_service.add_child(eventsub)
	var media_loader := TwitchMediaLoader.new()
	media_loader.name = "MediaLoader"
	media_loader.api = api
	twitch_service.add_child(media_loader)
	add_child(twitch_service)

func _setup_eventsub() -> void:
	for event in EVENTSUB_EVENTS:
		var conditions = {}
		for condition in event.conditions:
			conditions[condition] = broadcaster.id
		twitch_service.subscribe_event(event, conditions)

func _get_custom_rewards() -> void:
	var awards := await twitch_service.api.get_custom_reward([], false)
	var conditions = {"broadcaster_user_id": broadcaster.id}
	for reward : TwitchCustomReward in awards.data:
		conditions["reward_id"] = reward.id
		twitch_service.subscribe_event(TwitchEventsubDefinition.CHANNEL_CHANNEL_POINTS_CUSTOM_REWARD_REDEMPTION_ADD, conditions)
		print("(", reward.id, ") ", reward.title)
