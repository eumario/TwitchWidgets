using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.subscription.gift
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class SubscriptionGiftedEvent
{
    [JsonProperty("user_id")] public string UserId;
    [JsonProperty("user_login")] public string UserLogin;
    [JsonProperty("user_name")] public string UserName;
    [JsonProperty("broadcaster_user_id")] public string BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string BroadcasterUserName;
    [JsonProperty("total")] public int Total;
    [JsonProperty("tier")] public string Tier;
    [JsonProperty("cumulative_total")] public int? CumulativeTotal;
    [JsonProperty("is_anonymous")] public bool IsAnonymous;
}