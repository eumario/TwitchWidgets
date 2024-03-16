using Godot;

namespace TwitchWidgetsApp.Library;

[Tool]
[GlobalClass]
public partial class AlertSet : Resource
{
    [Export] public string AlertSetName;
    [Export] public PackedScene ChannelPointRedemptionAlert;
    [Export] public PackedScene CheerAlert;
    [Export] public PackedScene FollowAlert;
    [Export] public PackedScene SubscriptionGiftedAlert;
    [Export] public PackedScene NewChatterAlert;
    [Export] public PackedScene RaidAlert;
    [Export] public PackedScene SubscriptionAlert;
    [Export] public PackedScene SubscriptionMessageAlert;
}