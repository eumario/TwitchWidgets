using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.poll.begin
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class PollBeginEvent
{
    [JsonProperty("id")] public string? Id;
    [JsonProperty("broadcaster_user_id")] public string? BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string? BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string? BroadcasterName;
    [JsonProperty("title")] public string? Title;
    [JsonProperty("choices")] public List<PollChoice?>? Choices;
    [JsonProperty("bits_voting")] public VotingCurrency? BitsVoting;
    [JsonProperty("channel_points_voting")] public VotingCurrency? ChannelPointsVoting;
    [JsonProperty("started_at")] public string? StartedAt;
    [JsonProperty("ends_at")] public string? EndsAt;
}

public class PollChoice
{
    [JsonProperty("id")] public string? Id;
    [JsonProperty("title")] public string? Title;
}

public class VotingCurrency
{
    [JsonProperty("is_enabled")] public bool IsEnabled;
    [JsonProperty("amount_per_vote")] public int Amount;
}