using StreamingClient.Base.Util;

namespace TwitchWidgetsApp.Library.EventSub;

public enum SubscribeEnum
{
    
    [Name("channel.channel_points_custom_reward_redemption.add")]
    ChannelPointsRedeem,
    
    [Name("channel.cheer")]
    ChannelCheer,
    
    [Name("channel.follow")]
    ChannelFollow,
    
    [Name("channel.poll.begin")]
    PollBegin,
    
    [Name("channel.poll.progress")]
    PollProgress,
    
    [Name("channel.poll.end")]
    PollEnd,
    
    [Name("channel.prediction.begin")]
    PredictionBegin,
    
    [Name("channel.prediction.progress")]
    PredictionProgress,
    
    [Name("channel.prediction.lock")]
    PredictionLock,
    
    [Name("channel.prediction.end")]
    PredictionEnded,
    
    [Name("channel.raid")]
    ChannelRaid,
    
    [Name("channel.subscribe")]
    ChannelSubscription,
    
    [Name("channel.subscription.gift")]
    ChannelSubscriptionGifted,
    
    [Name("channel.subscription.message")]
    ChannelSubscriptionMessage
}