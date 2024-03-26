using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.prediction.lock
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class PredictionLockEvent
{
    [JsonProperty("id")] public string? Id;
    [JsonProperty("broadcaster_user_id")] public string? BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string? BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string? BroadcasterName;
    [JsonProperty("title")] public string? Title;
    [JsonProperty("outcomes")] public List<OutcomeOptionResults?>? Outcomes;
    [JsonProperty("started_at")] public string? StartedAt;
    [JsonProperty("locked_at")] public string? LockedAt;
}