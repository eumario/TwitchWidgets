using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;


//  Type: channel.subscription.message
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class SubscriptionMessageEvent
{
    [JsonProperty("user_id")] public string? UserId;
    [JsonProperty("user_login")] public string? UserLogin;
    [JsonProperty("user_name")] public string? UserName;
    [JsonProperty("broadcaster_user_id")] public string? BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string? BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string? BroadcasterUserName;
    [JsonProperty("tier")] public string? Tier;
    [JsonProperty("message")] public SubscriptionMessage? Message;
    [JsonProperty("cumulative_months")] public int CumulativeMonths;
    [JsonProperty("streak_months")] public int? StreakMonths;
    [JsonProperty("duration_months")] public int DurationMonths;
}

public class SubscriptionMessage
{
    [JsonProperty("text")] public string? Text;
    [JsonProperty("emotes")] public List<EmoteSubstitution?>? Emotes;
}

public class EmoteSubstitution
{
    [JsonProperty("begin")] public int Start;
    [JsonProperty("end")] public int End;
    [JsonProperty("id")] public string? Id;
}