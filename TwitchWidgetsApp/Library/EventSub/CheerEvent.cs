using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.cheer
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class CheerEvent
{
    [JsonProperty("is_anonymous")] public bool IsAnonymous;
    [JsonProperty("user_id")] public string? UserId;
    [JsonProperty("user_login")] public string? UserLogin;
    [JsonProperty("user_name")] public string? UserName;
    [JsonProperty("broadcaster_user_id")] public string BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string BroadcasterUserName;
    [JsonProperty("message")] public string Message;
    [JsonProperty("bits")] public int Bits;
}