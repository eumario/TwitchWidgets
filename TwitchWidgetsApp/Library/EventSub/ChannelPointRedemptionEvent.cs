using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.channel_points_custom_reward_redemption.add
//  Version: 1
//  Conditions:
//      broadcaster_user_id
//      reward_id

public class ChannelPointRedemptionEvent
{
    [JsonProperty("id")] public string? Id;
    [JsonProperty("broadcaster_user_id")] public string? BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string? BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string? BroadcasterUserName;
    [JsonProperty("user_id")] public string? UserId;
    [JsonProperty("user_login")] public string? UserLogin;
    [JsonProperty("user_name")] public string? UserName;
    [JsonProperty("user_input")] public string? UserInput;
    [JsonProperty("status")] public string? Status;
    [JsonProperty("reward")] public Reward? Reward;
    [JsonProperty("redeemed_at")] public string? RedeemedAt;
}

public class Reward
{
    [JsonProperty("id")] public string? Id;
    [JsonProperty("title")] public string? Title;
    [JsonProperty("cost")] public int Cost;
    [JsonProperty("prompt")] public string? Prompt;
}