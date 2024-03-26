using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.subscribe
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class SubscriptionEvent
{
    [JsonProperty("user_id")] public string? UserId;
    [JsonProperty("user_login")] public string? UserLogin;
    [JsonProperty("user_name")] public string? UserName;
    [JsonProperty("broadcaster_user_id")] public string? BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string? BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string? BroadcasterUserName;
    [JsonProperty("tier")] public string? Tier;
    [JsonProperty("is_gift")] public bool IsGift;
}