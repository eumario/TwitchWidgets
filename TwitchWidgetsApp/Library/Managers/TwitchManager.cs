using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Sharp.Extras;
using StreamingClient.Base.Model.OAuth;
using Twitch.Base;
using Twitch.Base.Clients;
using Twitch.Base.Models.Clients.Chat;
using Twitch.Base.Models.NewAPI.Users;
using TwitchWidgets.Data.Models;
using TwitchWidgetsApp.Library.EventSub;

namespace TwitchWidgetsApp.Library.Managers;

public partial class TwitchManager : Node
{

    [Signal] public delegate void ManagerReadyEventHandler();
    [Signal] public delegate void NewConnectionEventHandler();
    [Signal] public delegate void EventSubReconnectEventHandler();
    
    [Singleton] public Globals Globals;
    public TwitchConnection Connection;
    private TwitchConnection _botConnection;
    public UserModel Streamer;
    public UserModel Bot;
    public EventSub.EventSub EventSub;
    public ChatClient Chat;
    public bool IsReady = false;
    public bool IsChatConnected => Chat.IsOpen();
    public bool IsEventSubConnected => EventSub.IsOpen();

    private bool _isCurrentlyMock = false;

    private RandomNumberGenerator _rng;
    
    private List<Color> _twitchColors =
    [
        Colors.Blue,
        Colors.DodgerBlue,
        Colors.OrangeRed,
        Colors.BlueViolet,
        Colors.Firebrick,
        Colors.Red,
        Colors.CadetBlue,
        Colors.Goldenrod,
        Colors.SeaGreen,
        Colors.Chocolate,
        Colors.Green,
        Colors.SpringGreen,
        Colors.Coral,
        Colors.HotPink,
        Colors.YellowGreen
    ];

    public override void _Ready()
    {
        this.OnReady();
        _rng = new RandomNumberGenerator();
        _rng.Randomize();
        Globals.CredentialsUpdated += LoadConnectionInfo;
        Globals.SettingsLoaded += GlobalsOnSettingsLoaded;
        Globals.SettingsUpdated += GlobalsOnSettingsUpdated;
    }

    private void GlobalsOnSettingsUpdated()
    {
        if (Globals.UseMockServer == _isCurrentlyMock) return;
        
        var currentEventSub = EventSub;
        EventSub = new EventSub.EventSub(Connection, Globals.UseMockServer);
        _isCurrentlyMock = Globals.UseMockServer;
        
        if (!currentEventSub.IsOpen()) return;
        
        currentEventSub.Disconnect();
        EventSub.Connect();
        EventSub.OnConnected += (sender, args) =>
        {
            EventSub.Subscribe([
                SubscribeEnum.ChannelRaid, SubscribeEnum.ChannelSubscription, SubscribeEnum.ChannelFollow,
                SubscribeEnum.ChannelCheer, SubscribeEnum.ChannelSubscriptionGifted,
                SubscribeEnum.ChannelSubscriptionMessage
            ], Streamer.id);
        };
    }

    private async void GlobalsOnSettingsLoaded()
    {
        LoadConnectionInfo();
        if (Globals.AutoConnectTwitch)
        {
            while (!IsReady)
            {
                await Task.Delay(500);
            }
            ConnectTwitch();
        }
    }

    private async void LoadConnectionInfo()
    {
        if (Globals.Database == null) { Globals.RunOnMain(LoadConnectionInfo); return; }
        if (Globals.Database.Secrets == null) { Globals.RunOnMain(LoadConnectionInfo); return; }

        var res = Globals.Database.Secrets.ToList();
        if (res.Count == 0) return;
        var secret = res[0];
        var streamerToken = new OAuthTokenModel()
        {
            clientID = secret.ClientId,
            clientSecret = secret.ClientSecret,
            accessToken = secret.StreamerAuthToken,
            refreshToken = secret.StreamerRefreshToken,
        };
        var botToken = new OAuthTokenModel()
        {
            clientID = secret.ClientId,
            clientSecret = secret.ClientSecret,
            accessToken = secret.BotAuthToken,
            refreshToken = secret.BotRefreshToken,
        };
        Connection = await TwitchConnection.ConnectViaOAuthToken(streamerToken);
        _botConnection = await TwitchConnection.ConnectViaOAuthToken(botToken);

        Streamer = await Connection.NewAPI.Users.GetCurrentUser();
        Bot = await _botConnection.NewAPI.Users.GetCurrentUser();

        UpdateTokensAsNeeded(secret, streamerToken, botToken);
        
        Chat = new ChatClient(_botConnection);
        Chat.OnMessageReceived += ChatOnMessageReceived;
        EventSub = new EventSub.EventSub(Connection, Globals.UseMockServer);
        EventSub.OnReconnectRequested += EventSubOnReconnectRequested;
        _isCurrentlyMock = Globals.UseMockServer;
        EmitSignal(SignalName.NewConnection);
        IsReady = true;
        EmitSignal(SignalName.ManagerReady);
    }

    private async void ChatOnMessageReceived(object sender, ChatMessagePacketModel e)
    { 
        var user = Globals.Chatters.FirstOrDefault(x => x.id == e.UserID);
        if (user != null)
        {
            var kkc = Globals.KnownChatters.FirstOrDefault(x => x.TwitchId == e.UserID);
            if (kkc == null)
            {
                GD.PrintErr("Chatter not saved in Database: " + e.UserID);
                return;
            }
            kkc.LastSeen = DateTime.UtcNow;
            await Globals.Database.SaveChangesAsync();
            return;
        }
        
        user = await Globals.TwitchManager.Connection.NewAPI.Users.GetUserByID(e.UserID);

        if (user == null) return;
        
        Globals.Chatters.Add(user);
        var color = Color.FromHtml(e.Color);
        if (color == Colors.Black)
        {
            var indx = (int)_rng.Randi() % _twitchColors.Count;
            if (indx < 0) indx *= -1;
            color = _twitchColors[indx];
            Globals.SavedChatColors[user] = color;
        }
        Globals.SavedChatColors[user] = color;

        var kc = Globals.Database.KnownChatters!.FirstOrDefault(x => x.TwitchId == user.id);
        if (kc == null)
        {
            kc = new KnownChatter()
            {
                TwitchId = user.id,
                DisplayName = user.display_name,
                AvatarUrl = user.profile_image_url,
                StreamAvatar = "",
                FirstSeen = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow
            };
            Globals.Database.KnownChatters.Add(kc);
            await Globals.Database.SaveChangesAsync();
        }
        else
        {
            kc.LastSeen = DateTime.UtcNow;
        }

        Globals.KnownChatters.Add(kc);
        
        Globals.ChatHistory.Add(e);
        
        Globals.AlertManager.NewChatterAlert(user, e);
    }

    private void EventSubOnReconnectRequested(object sender, string e)
    {
        GD.Print($"Reconnect message came through, Reconnect URL: {e}");
        var oldEventSub = EventSub;
        EventSub = new EventSub.EventSub(Connection, Globals.UseMockServer, true);
        _isCurrentlyMock = Globals.UseMockServer;
        EmitSignal(SignalName.EventSubReconnect);
        EventSub.Connect(e);
        if (oldEventSub.IsOpen())
            oldEventSub.Disconnect();
    }

    private void UpdateTokensAsNeeded(Secret secret, OAuthTokenModel streamer, OAuthTokenModel bot)
    {
        var anyChanges = false;
        if (secret.StreamerAuthToken != streamer.accessToken)
        {
            secret.StreamerAuthToken = streamer.accessToken;
            secret.StreamerRefreshToken = streamer.refreshToken;
            anyChanges = true;
        }

        if (secret.BotAuthToken != bot.accessToken)
        {
            secret.BotAuthToken = bot.accessToken;
            secret.BotRefreshToken = bot.refreshToken;
            anyChanges = true;
        }

        if (!anyChanges) return;
        Globals.Database.SaveChanges();
    }

    public async void ConnectTwitch()
    {
        if (!IsReady) return;

        // Chat Client Stuff
        Chat.OnPingReceived += (sender, args) =>
        {
            GD.Print("Ping? PONG!");
            Chat.Pong();
        };
        Chat.OnGlobalUserStateReceived += (sender, model) => Chat.Join(Streamer);
        Chat.OnUserListReceived += (sender, model) => Chat.SendMessage(Streamer, $"{Bot.display_name} is now online.");
        Chat.Connect();

        EventSub.Connect();
        EventSub.OnConnected += (sender, args) =>
        {
            EventSub.Subscribe([
                SubscribeEnum.ChannelRaid, SubscribeEnum.ChannelSubscription, SubscribeEnum.ChannelFollow,
                SubscribeEnum.ChannelCheer, SubscribeEnum.ChannelSubscriptionGifted,
                SubscribeEnum.ChannelSubscriptionMessage
            ], Streamer.id);
        };
    }

    public void DisconnectTwitch()
    {
        // Chat Client Stuff
        Chat.Disconnect();
        Chat = new ChatClient(_botConnection);
        
        // Event Sub Stuff
        EventSub.Disconnect();
        EventSub = new EventSub.EventSub(Connection, Globals.UseMockServer);
        _isCurrentlyMock = Globals.UseMockServer;
        EventSub.OnReconnectRequested += EventSubOnReconnectRequested;
        
        EmitSignal(SignalName.NewConnection);
    }
}