using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.poll.progress
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class PollProgressEvent
{
    [JsonProperty("id")] public string? Id;
    [JsonProperty("broadcaster_user_id")] public string? BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string? BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string? BroadcasterName;
    [JsonProperty("title")] public string? Title;
    [JsonProperty("choices")] public List<PollChoiceResults?>? Choices;
    [JsonProperty("bits_voting")] public VotingCurrency? BitsVoting;
    [JsonProperty("channel_points_voting")] public VotingCurrency? ChannelPointsVoting;
    [JsonProperty("started_at")] public string? StartedAt;
    [JsonProperty("ends_at")] public string? EndsAt;
}

public class PollChoiceResults : PollChoice
{
    [JsonProperty("bits_votes")] public int BitsVotes;
    [JsonProperty("channel_points_votes")] public int ChannelPointsVotes;
    [JsonProperty("votes")] public int Votes;
}