using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchWidgetsApp.Library.EventSub;

//  Type: channel.poll.end
//  Version: 1
//  Conditions:
//      broadcaster_user_id

public class PollEndEvent
{
    [JsonProperty("id")] public string Id;
    [JsonProperty("broadcaster_user_id")] public string BroadcasterId;
    [JsonProperty("broadcaster_user_login")] public string BroadcasterLogin;
    [JsonProperty("broadcaster_user_name")] public string BroadcasterName;
    [JsonProperty("title")] public string Title;
    [JsonProperty("choices")] public List<PollChoiceResults> Choices;
    [JsonProperty("bits_voting")] public VotingCurrency BitsVoting;
    [JsonProperty("channel_points_voting")] public VotingCurrency ChannelPointsVoting;
    [JsonProperty("status")] public string Status;
    [JsonProperty("started_at")] public string StartedAt;
    [JsonProperty("ended_at")] public string EndsAt;
}