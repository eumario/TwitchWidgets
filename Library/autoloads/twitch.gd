class_name TwitchManager extends Node

#region Signals
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
#endregion

#region Enumerations
enum State {
	BEGIN,
	PROGRESS,
	LOCK,
	END
}
#endregion

#region Support Classes
class Twitcher extends RefCounted:
	var user : TwitchUser
	var service : TwitchService
	var api : TwitchAPI :
		get(): return service.api
	var eventsub : TwitchEventsub :
		get(): return service.eventsub
	var auth : TwitchAuth :
		get(): return service.auth
	var media_loader : TwitchMediaLoader:
		get(): return service.media_loader
	var name : String :
		get(): return name
		set(value):
			name = value
			service.name = name + "-TwitchService"
			if service.api: service.api.name = name + "-Api"
			if service.auth: service.auth.name = name + "-Auth"
			if service.eventsub: service.eventsub.name = name + "-EventSub"
			if service.media_loader: service.media_loader.name = name + "-MediaLoader"
			
	
	func _init(oauth : OAuthSetting, scopes : OAuthScopes, token : OAuthToken, is_bot : bool = false) -> void:
		service = TwitchService.new()
		service.oauth_setting = oauth
		service.scopes = scopes # load("res://Resources/twitch_scopes.tres")
		service.token = token # load("res://Resources/streamer_token.tres")
		var napi := TwitchAPI.new()
		service.add_child(napi)
		if not is_bot:
			var neventsub := TwitchEventsub.new()
			neventsub.api = napi
			service.eventsub = neventsub
			service.add_child(neventsub)
		var nmedia_loader := TwitchMediaLoader.new()
		nmedia_loader.api = napi
		service.add_child(nmedia_loader)
		name = "Twitcher"
	
	func get_user() -> bool:
		var info : TwitchGetUsersResponse = await service.api.get_users([], [])
		if info.data.size() > 0:
			user = info.data[0]
			return true
		return false
	
	func authorize() -> bool:
		auth.force_verify = true
		await auth.authorize()
		user = await service.get_current_user()
		return auth.is_authenticated
	
	func login() -> bool:
		auth.force_verify = false
		await service.setup()
		user = await service.get_current_user()
		return auth.is_authenticated
#endregion

#region Properties
var broadcaster : Twitcher
var bot : Twitcher
#endregion

#region EventSub Events Array
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
#endregion

#region Godot Overrides
func _ready() -> void:
	Managers.init_finished.connect(func():
		var settings = Managers.settings.data
		if settings.client_id == "" and settings.client_secret == "": return
		
		setup_streamer_auth(settings.client_id, settings.client_secret)
		setup_bot_auth(settings.client_id, settings.client_secret)
		
		if broadcaster.auth.is_authenticated:
			await broadcaster.get_user()
		
		if bot.auth.is_authenticated:
			await bot.get_user()
		
		if settings.auto_connect_twitch:
			if broadcaster: await broadcaster.login()
			if bot: await bot.login()
	)
#endregion

#region Signal Handler functions
func _enable_events() -> void:
	broadcaster.eventsub.event.connect(_handle_eventsub)

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
#endregion

#region Private Support Functions
func _setup_broadcaster_nodes(oauth : OAuthSetting) -> void:
	broadcaster = Twitcher.new(
		oauth,
		load("res://Resources/twitch/twitch_scopes.tres"),
		load("res://Resources/twitch/streamer_token.tres")
	)
	add_child(broadcaster.service)
	await get_tree().process_frame
	broadcaster.name = "Broadcaster"

func _setup_bot_nodes(oauth : OAuthSetting) -> void:
	bot = Twitcher.new(
		oauth,
		load("res://Resources/twitch/chat_scopes.tres"),
		load("res://Resources/twitch/bot_token.tres"),
		true
	)
	add_child(bot.service)
	await get_tree().process_frame
	bot.name = "Bot"

func _setup_eventsub() -> void:
	for event in EVENTSUB_EVENTS:
		var conditions = {}
		for condition in event.conditions:
			conditions[condition] = broadcaster.user.id
		broadcaster.service.subscribe_event(event, conditions)

func _get_custom_rewards() -> void:
	var awards := await broadcaster.service.api.get_custom_reward([], false)
	var conditions = {"broadcaster_user_id": broadcaster.user.id}
	for reward : TwitchCustomReward in awards.data:
		conditions["reward_id"] = reward.id
		broadcaster.service.subscribe_event(TwitchEventsubDefinition.CHANNEL_CHANNEL_POINTS_CUSTOM_REWARD_REDEMPTION_ADD, conditions)
		print("(", reward.id, ") ", reward.title)

func _setup_eventsub_stuff(_id : String) -> void:
	_setup_eventsub()
	_get_custom_rewards()
	broadcaster.eventsub.session_id_received.disconnect(_setup_eventsub_stuff)
#endregion

#region Public API
func send_chat_message(message : String) -> void:
	var body := TwitchSendChatMessageBody.new()
	body.broadcaster_id = broadcaster.user.id
	body.sender_id = broadcaster.user.id
	body.message = message
	
	Logger.debug("Sending Message: %s> %s" % [broadcaster.user.login, message])
	var _res : TwitchSendChatMessageResponse = await broadcaster.service.api.send_chat_message(body)

func send_reply_message(id : String, message : String) -> void:
	var body := TwitchSendChatMessageBody.new()
	body.broadcaster_id = broadcaster.user.id
	body.sender_id = broadcaster.user.id
	body.message = message
	body.reply_parent_message_id = id
	
	Logger.debug("Sending Reply Message: %s:(%s)> %s" % [broadcaster.user.login, id, message])
	var _res : TwitchSendChatMessageResponse = await broadcaster.service.api.send_chat_message(body)

func send_bot_message(message : String) -> void:
	var body := TwitchSendChatMessageBody.new()
	body.broadcaster_id = broadcaster.user.id
	body.sender_id = bot.user.id
	body.message = message
	
	Logger.debug("Bot Sending Message: %s> %s" % [bot.user.login, message])
	var _res : TwitchSendChatMessageResponse = await bot.service.api.send_chat_message(body)

func send_bot_reply_message(id : String, message : String) -> void:
	var body := TwitchSendChatMessageBody.new()
	body.broadcaster_id = broadcaster.user.id
	body.sender_id = bot.user.id
	body.message = message
	body.reply_parent_message_id = id
	
	Logger.debug("Sending Reply Message: %s:(%s)> %s" % [bot.user.login, id, message])
	var _res : TwitchSendChatMessageResponse = await bot.service.api.send_chat_message(body)

func setup_streamer_auth(client_id : String = "", client_secret : String = "") -> void:
	var oauth : OAuthSetting = load("res://addons/twitcher/default_oauth_setting.tres").duplicate()
	
	oauth.client_id = client_id
	oauth.client_secret = client_secret
	
	await _setup_broadcaster_nodes(oauth)
	_enable_events()
	await get_tree().process_frame
	
	broadcaster.eventsub.session_id_received.connect(_setup_eventsub_stuff)

func setup_bot_auth(client_id : String = "", client_secret : String = "") -> void:
	var oauth : OAuthSetting = load("res://addons/twitcher/default_oauth_setting.tres").duplicate()
	
	oauth.client_id = client_id
	oauth.client_secret = client_secret
	
	_setup_bot_nodes(oauth)
	await get_tree().process_frame
	
#endregion
