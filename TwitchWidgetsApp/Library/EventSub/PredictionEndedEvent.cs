using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.prediction.end
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class PredictionEndedEvent
{
    [JsonProperty("id")] public string Id;
    [JsonProperty("broadcaster_user_id")] public string BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string BroadcasterName;
    [JsonProperty("title")] public string Title;
    [JsonProperty("winning_outcome_id")] public string OutcomeId;
    [JsonProperty("outcomes")] public List<OutcomeOptionResults> Outcomes;
    [JsonProperty("status")] public string Status;
    [JsonProperty("started_at")] public string StartedAt;
    [JsonProperty("ended_at")] public string LockedAt;
}