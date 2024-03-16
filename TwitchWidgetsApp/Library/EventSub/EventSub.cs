using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using StreamingClient.Base.Util;
using Swan.Logging;
using Twitch.Base;
using Twitch.Base.Clients;
using Twitch.Base.Models.Clients.EventSub;
using StringExtensions = Swan.StringExtensions;

namespace TwitchWidgetsApp.Library.EventSub;

public class EventSub
{
    private EventSubClient _eventSubClient;
    private TwitchConnection _connection;

    private string _sessionId;
    private int? _keepAliveTimeout;
    private string _reconnectUrl;
    private bool _useMockServer;
    private bool _isReconnect;
    public Dictionary<SubscribeEnum, (Type, MethodInfo)> _messageTypeMap;
    
    public event EventHandler<ChannelPointRedemptionEvent> OnChannelPointRedemption;
    public event EventHandler<CheerEvent> OnCheer;
    public event EventHandler<FollowEvent> OnFollow;
    public event EventHandler<PollBeginEvent> OnPollBegin;
    public event EventHandler<PollProgressEvent> OnPollProgress;
    public event EventHandler<PollEndEvent> OnPollEnded;
    public event EventHandler<PredictionBeginEvent> OnPredictionBegin;
    public event EventHandler<PredictionProgressEvent> OnPredictionProgress;
    public event EventHandler<PredictionLockEvent> OnPredictionLock;
    public event EventHandler<PredictionEndedEvent> OnPredictionEnded;
    public event EventHandler<RaidEvent> OnRaid;
    public event EventHandler<SubscriptionEvent> OnSubscription;
    public event EventHandler<SubscriptionGiftedEvent> OnSubscriptionGifted;
    public event EventHandler<SubscriptionMessageEvent> OnSubscriptionMessage;
    public event EventHandler<string> OnReconnectRequested;
    public event EventHandler OnConnected;
    

    public EventSub(TwitchConnection connection, bool useMockServer = false, bool isReconnect = false)
    {
        _connection = connection;
        _useMockServer = useMockServer;
        _isReconnect = isReconnect;
        _eventSubClient = new EventSubClient();
        _eventSubClient.OnWelcomeMessageReceived += EventSubClientOnWelcomeMessageReceived;
        _eventSubClient.OnNotificationMessageReceived += EventSubClientOnNotificationMessageReceived;
        _eventSubClient.OnKeepAliveMessageReceived += EventSubClientOnKeepAliveMessageReceived;
        _eventSubClient.OnRevocationMessageReceived += EventSubClientOnRevocationMessageReceived;
        _eventSubClient.OnReconnectMessageReceived += EventSubClientOnReconnectMessageReceived;

        InitMessageMap();
    }

    private void InitMessageMap()
    {
        IReadOnlyDictionary<Type, MethodInfo> messageHandlers = typeof(EventSub)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(m => m.Name == "HandleMessage")
            .ToDictionary(e => e.GetParameters().First().ParameterType, e => e);

        _messageTypeMap = new Dictionary<SubscribeEnum, (Type, MethodInfo)>()
        {
            { SubscribeEnum.ChannelPointsRedeem, (typeof(ChannelPointRedemptionEvent), messageHandlers[typeof(ChannelPointRedemptionEvent)]) },
            { SubscribeEnum.ChannelCheer, (typeof(CheerEvent), messageHandlers[typeof(CheerEvent)]) },
            { SubscribeEnum.ChannelFollow, (typeof(FollowEvent), messageHandlers[typeof(FollowEvent)]) },
            { SubscribeEnum.PollBegin, (typeof(PollBeginEvent), messageHandlers[typeof(PollBeginEvent)]) },
            { SubscribeEnum.PollProgress, (typeof(PollProgressEvent), messageHandlers[typeof(PollProgressEvent)]) },
            { SubscribeEnum.PollEnd, (typeof(PollEndEvent), messageHandlers[typeof(PollEndEvent)]) },
            { SubscribeEnum.PredictionBegin, (typeof(PredictionBeginEvent), messageHandlers[typeof(PredictionBeginEvent)]) },
            { SubscribeEnum.PredictionProgress, (typeof(PredictionProgressEvent), messageHandlers[typeof(PredictionProgressEvent)]) },
            { SubscribeEnum.PredictionLock, (typeof(PredictionLockEvent), messageHandlers[typeof(PredictionLockEvent)]) },
            { SubscribeEnum.PredictionEnded, (typeof(PredictionEndedEvent), messageHandlers[typeof(PredictionEndedEvent)]) },
            { SubscribeEnum.ChannelRaid, (typeof(RaidEvent), messageHandlers[typeof(RaidEvent)]) },
            { SubscribeEnum.ChannelSubscription, (typeof(SubscriptionEvent), messageHandlers[typeof(SubscriptionEvent)]) },
            { SubscribeEnum.ChannelSubscriptionGifted, (typeof(SubscriptionGiftedEvent), messageHandlers[typeof(SubscriptionGiftedEvent)]) },
            { SubscribeEnum.ChannelSubscriptionMessage, (typeof(SubscriptionMessageEvent), messageHandlers[typeof(SubscriptionMessageEvent)]) }
        };
    }
    
    public bool IsOpen() => _eventSubClient.IsOpen();
    public async void Disconnect() => await _eventSubClient.Disconnect();
    public async void Connect(string url = "")
    {
        if (_useMockServer)
            await _eventSubClient.Connect(string.IsNullOrEmpty(url) ? "ws://localhost:4090/ws" : url);
        else if (!string.IsNullOrEmpty(url))
            await _eventSubClient.Connect(url);
        else
            await _eventSubClient.Connect();
        Console.WriteLine($"Connected at: {DateTime.Now:MM/dd/yy h:mm:ss tt}");
    }

    public async Task Subscribe(List<SubscribeEnum> topics, string userId)
    {
        if (_useMockServer) return;
        if (_isReconnect) return;
        foreach (var topic in topics)
        {
            GD.Print($"Subscribing to {topic}: {EnumHelper.GetEnumName(topic)}");
            GD.Print($"Websocket Session ID: {_sessionId}");
            var conditions = new Dictionary<string, string>();
            var version = "1";
            switch (topic)
            {
                case SubscribeEnum.ChannelRaid:
                    conditions["to_broadcaster_user_id"] = userId;
                    break;
                case SubscribeEnum.ChannelFollow:
                    conditions["broadcaster_user_id"] = userId;
                    conditions["moderator_user_id"] = userId;
                    version = "2";
                    break;
                default:
                    conditions["broadcaster_user_id"] = userId;
                    break;
            }
            
            GD.Print($"Broadcaster User ID: {userId}");
            if (topic == SubscribeEnum.ChannelFollow)
                GD.Print($"Moderator User ID: {userId}");

            try
            {
                var result = await _connection.NewAPI.EventSub.CreateSubscription(EnumHelper.GetEnumName(topic),
                    "websocket", conditions,
                    _sessionId, version: version);
                GD.Print(
                    $"Result: {result.created_at}, Status: {result.status}, Transport: {result.transport.method}, Type: {result.type}");
            }
            catch (Exception ex)
            {
                GD.Print($"Exception Occurred: {ex.Message}");
            }
        }
    }

    public async Task SubscribeRedemptions(string userId, List<string> rewardIds)
    {
        if (_useMockServer) return;
        var conditions = new Dictionary<string, string>()
        {
            { "broadcaster_user_id", userId }
        };

        foreach (var rewardId in rewardIds)
        {
            conditions["reward_id"] = rewardId;
            await _connection.NewAPI.EventSub.CreateSubscription(
                EnumHelper.GetEnumName(SubscribeEnum.ChannelPointsRedeem), "websocket", conditions, _sessionId);
        }
    }

    private void EventSubClientOnWelcomeMessageReceived(object sender, WelcomeMessage e)
    {
        Console.WriteLine($"Welcome Message Received at: {DateTime.Now:MM/dd/yy h:mm:ss tt}");
        if (_isReconnect)
        {
            Console.WriteLine($"Welcome Packet received on Reconnect, ID: {e.Payload.Session.Id}");
        }
        
        Console.WriteLine($"Reconnect URL: {e.Payload.Session.ReconnectUrl}");
        
        _sessionId = e.Payload.Session.Id;
        _keepAliveTimeout = e.Payload.Session.KeepaliveTimeoutSeconds;
        _reconnectUrl = e.Payload.Session.ReconnectUrl;
        OnConnected?.Invoke(this, EventArgs.Empty);
    }

    private void EventSubClientOnNotificationMessageReceived(object sender, NotificationMessage e)
    {
        var sub = _messageTypeMap.Keys.FirstOrDefault(x => EnumHelper.GetEnumName(x) == e.Payload.Subscription.Type);
        if (sub == null) return;
        var payload = e.Payload.Event.ToObject(_messageTypeMap[sub].Item1);
        _messageTypeMap[sub].Item2.Invoke(this, [payload]);
    }

    private void EventSubClientOnKeepAliveMessageReceived(object sender, KeepAliveMessage e)
    {
        // Noop
    }

    private void EventSubClientOnRevocationMessageReceived(object sender, RevocationMessage e)
    {
        // Noop
    }

    private void EventSubClientOnReconnectMessageReceived(object sender, ReconnectMessage e)
    {
        Console.WriteLine($"Reconnect Message Received at: {DateTime.Now:MM/dd/yy h:mm:ss tt}");
        OnReconnectRequested?.Invoke(this, e.Payload.Session.ReconnectUrl);
    }
    
    private Task HandleMessage(ChannelPointRedemptionEvent message)
    {
        OnChannelPointRedemption?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(CheerEvent message)
    {
        OnCheer?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(FollowEvent message)
    {
        OnFollow?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(PollBeginEvent message)
    {
        OnPollBegin?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(PollProgressEvent message)
    {
        OnPollProgress?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(PollEndEvent message)
    {
        OnPollEnded?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(PredictionBeginEvent message)
    {
        OnPredictionBegin?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(PredictionProgressEvent message)
    {
        OnPredictionProgress?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(PredictionLockEvent message)
    {
        OnPredictionLock?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(PredictionEndedEvent message)
    {
        OnPredictionEnded?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(RaidEvent message)
    {
        OnRaid?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(SubscriptionEvent message)
    {
        OnSubscription?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(SubscriptionGiftedEvent message)
    {
        OnSubscriptionGifted?.Invoke(this, message);
        return Task.CompletedTask;
    }
    
    private Task HandleMessage(SubscriptionMessageEvent message)
    {
        OnSubscriptionMessage?.Invoke(this, message);
        return Task.CompletedTask;
    }
}