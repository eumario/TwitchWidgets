using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.prediction.progress
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class PredictionProgressEvent
{
    [JsonProperty("id")] public string? Id;
    [JsonProperty("broadcaster_user_id")] public string? BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string? BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string? BroadcasterName;
    [JsonProperty("title")] public string? Title;
    [JsonProperty("outcomes")] public List<OutcomeOptionResults?>? Outcomes;
    [JsonProperty("started_at")] public string? StartedAt;
    [JsonProperty("locks_at")] public string? LocksAt;
}

public class OutcomeOptionResults : OutcomeOptions
{
    [JsonProperty("users")] public int Users;
    [JsonProperty("channel_points")] public int Points;
    [JsonProperty("top_predictors")] public List<OutcomePredictor?>? TopPredictors;
}

public class OutcomePredictor
{
    [JsonProperty("user_id")] public string? UserId;
    [JsonProperty("user_login")] public string? UserLogin;
    [JsonProperty("user_name")] public string? UserName;
    [JsonProperty("channel_points_won")] public bool? Won;
    [JsonProperty("channel_points_used")] public int Points;
}