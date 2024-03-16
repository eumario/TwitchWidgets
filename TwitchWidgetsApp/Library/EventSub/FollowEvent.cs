using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.follow
//  Version: 2
//  Conditions:
//      broadcaster_user_id
//      moderator_user_id

public class FollowEvent
{
    [JsonProperty("user_id")] public string UserId;
    [JsonProperty("user_login")] public string UserLogin;
    [JsonProperty("user_name")] public string UserName;
    [JsonProperty("broadcaster_user_id")] public string BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string BroadcasterUserName;
    [JsonProperty("followed_at")] public string FollowedAt;
}