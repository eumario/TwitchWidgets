using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.prediction.begin
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class PredictionBeginEvent
{
    [JsonProperty("id")] public string? Id;
    [JsonProperty("broadcaster_user_id")] public string? BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string? BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string? BroadcasterName;
    [JsonProperty("title")] public string? Title;
    [JsonProperty("outcomes")] public List<OutcomeOptions?>? Outcomes;
    [JsonProperty("started_at")] public string? StartedAt;
    [JsonProperty("locks_at")] public string? LocksAt;
}

public class OutcomeOptions
{
    [JsonProperty("id")] public string? Id;
    [JsonProperty("title")] public string? Title;
    [JsonProperty("color")] public string? Color;
}