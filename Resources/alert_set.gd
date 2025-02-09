class_name AlertSet extends Resource

enum AlertType {
	ChannelPointRedemption,
	CheerAlert,
	FollowAlert,
	NewChatterAlert,
	RaidAlert,
	SubscriptionAlert,
	SubscriptionGiftedAlert,
	SubscriptionMessageAlert,
}

@export var set_name : String
@export var alerts : Dictionary[AlertType, PackedScene]
