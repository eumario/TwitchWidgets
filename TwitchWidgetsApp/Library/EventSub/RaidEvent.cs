using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.raid
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class RaidEvent
{
    [JsonProperty("from_broadcaster_user_id")] public string FromBroadcasterId;
    [JsonProperty("from_broadcaster_user_login")] public string FromBroadcasterLogin;
    [JsonProperty("from_broadcaster_user_name")] public string FromBroadcasterUserName;
    
    [JsonProperty("to_broadcaster_user_id")] public string ToBroadcasterId;
    [JsonProperty("to_broadcaster_user_login")] public string ToBroadcasterLogin;
    [JsonProperty("to_broadcaster_user_name")] public string ToBroadcasterUserName;

    [JsonProperty("viewers")] public int Viewers;
}