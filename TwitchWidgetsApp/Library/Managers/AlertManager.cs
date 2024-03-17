using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Sharp.Extras;
using Twitch.Base.Models.Clients.Chat;
using Twitch.Base.Models.NewAPI.Users;
using TwitchWidgetsApp.Library.EventSub;
using TwitchWidgetsApp.Resources.AlertCollections.ForestAlerts;

namespace TwitchWidgetsApp.Library.Managers;

public partial class AlertManager : Node
{
    [Singleton] public Globals Globals;
    [Signal] public delegate void ShowAlertEventHandler(AlertScript alert);
    public AlertSet CurrentSet = null;
    private Queue<AlertScript> _queuedAlerts = new();
    private AlertScript _currentAlert = null;

    public override void _Ready()
    {
        this.OnReady();
        Globals.TwitchManager.NewConnection += TwitchManagerOnNewConnection;
        Globals.SettingsLoaded += GlobalsOnSettingsLoaded;
    }

    private void GlobalsOnSettingsLoaded()
    {
        var path = Globals.SettingsManager.GetValue("alert_set", "");
        if (string.IsNullOrEmpty(path)) return;
        if (CurrentSet != null && CurrentSet.ResourcePath == path) return;
        CurrentSet = GD.Load<AlertSet>(path);
    }

    private void SetupEventSub()
    {
        Globals.TwitchManager.EventSub.OnCheer += EventSubOnCheer;
        Globals.TwitchManager.EventSub.OnFollow += EventSubOnFollow;
        Globals.TwitchManager.EventSub.OnRaid += EventSubOnRaid;
        Globals.TwitchManager.EventSub.OnSubscription += EventSubOnSubscription;
        Globals.TwitchManager.EventSub.OnSubscriptionGifted += EventSubOnSubscriptionGifted;
        Globals.TwitchManager.EventSub.OnSubscriptionMessage += EventSubOnSubscriptionMessage;
        
        //Globals.TwitchManager.EventSub.OnChannelPointRedemption += (sender, @event) =>
        //Globals.TwitchManager.EventSub.OnPollBegin += (sender, @event) =>
        //Globals.TwitchManager.EventSub.OnPollProgress += (sender, @event) =>
        //Globals.TwitchManager.EventSub.OnPollEnded += (sender, @event) =>
        //Globals.TwitchManager.EventSub.OnPredictionBegin += (sender, @event) =>
        //Globals.TwitchManager.EventSub.OnPredictionProgress += (sender, @event) =>
        //Globals.TwitchManager.EventSub.OnPredictionLock += (sender, @event) =>
        //Globals.TwitchManager.EventSub.OnPredictionEnded += (sender, @event) =>
    }

    private void EventSubOnSubscriptionMessage(object sender, SubscriptionMessageEvent e)
    {
        if (CurrentSet == null) return;
        var alert = CurrentSet.SubscriptionMessageAlert.Instantiate<AlertScript>();
        alert.Text = $"{e.UserName}\nRenewed Their Subscription\n{e.Message.Text}";
        _queuedAlerts.Enqueue(alert);
        alert.AlertFinished += PlayNextAlert;
        alert.AlertReady += CheckAlerts;
        EmitSignal(SignalName.ShowAlert, alert);
    }

    private void EventSubOnSubscriptionGifted(object sender, SubscriptionGiftedEvent e)
    {
        if (CurrentSet == null) return;
        var alert = CurrentSet.SubscriptionGiftedAlert.Instantiate<AlertScript>();
        alert.Text = $"{e.UserName}\nHas just Gifted\n{e.Total} Subs.";
        _queuedAlerts.Enqueue(alert);
        alert.AlertFinished += PlayNextAlert;
        alert.AlertReady += CheckAlerts;
        EmitSignal(SignalName.ShowAlert, alert);
    }

    private void EventSubOnSubscription(object sender, SubscriptionEvent e)
    {
        if (CurrentSet == null) return;
        var alert = CurrentSet.SubscriptionAlert.Instantiate<AlertScript>();
        alert.Text = $"{e.UserName}\nHas just Subscribed.";
        _queuedAlerts.Enqueue(alert);
        alert.AlertFinished += PlayNextAlert;
        alert.AlertReady += CheckAlerts;
        EmitSignal(SignalName.ShowAlert, alert);
    }

    private void EventSubOnRaid(object sender, RaidEvent e)
    {
        if (CurrentSet == null) return;
        var alert = CurrentSet.RaidAlert.Instantiate<AlertScript>();
        alert.Text = $"{e.FromBroadcasterUserName}\nHas Just Raided\nWith {e.Viewers} Viewers";
        _queuedAlerts.Enqueue(alert);
        alert.AlertFinished += PlayNextAlert;
        alert.AlertReady += CheckAlerts;
        EmitSignal(SignalName.ShowAlert, alert);
    }

    private void EventSubOnFollow(object sender, FollowEvent e)
    {
        if (CurrentSet == null) return;
        var alert = CurrentSet.FollowAlert.Instantiate<AlertScript>();
        alert.Text = $"{e.UserName}\nHas just followed.";
        _queuedAlerts.Enqueue(alert);
        alert.AlertFinished += PlayNextAlert;
        alert.AlertReady += CheckAlerts;
        EmitSignal(SignalName.ShowAlert, alert);
    }

    private async void EventSubOnCheer(object sender, CheerEvent e)
    {
        if (CurrentSet == null) return;
        var alert = CurrentSet.CheerAlert.Instantiate<AlertScript>();
        alert.Text = $"{e.UserName}\nHas Just Cheered\n{e.Bits} Bits";
        await CheckForTts(e.UserId, e.Message, e.Bits);
        Globals.TtsManager.AddTtsMessage(e.Message);
        _queuedAlerts.Enqueue(alert);
        alert.AlertFinished += PlayNextAlert;
        alert.AlertReady += CheckAlerts;
        EmitSignal(SignalName.ShowAlert, alert);
    }

    private async Task CheckForTts(string userId, string message, int bits)
    {
        var user = Globals.Chatters.FirstOrDefault(x => x.id == userId);
        if (user == null) {
            Globals.RunOnMain(() => CheckForTts(userId, message, bits));
            return;
        }

        var isSubbed =
            await Globals.TwitchApi.Subscriptions.GetSubscription(Globals.Streamer,
                user);
        if (isSubbed == null && bits >= 500)
            Globals.TtsManager.AddTtsMessage(message);
        else if (isSubbed != null && bits >= 100)
            Globals.TtsManager.AddTtsMessage(message);
    }

    public void NewChatterAlert(UserModel user, ChatMessagePacketModel message)
    {
        if (CurrentSet == null) return;
        var alert = CurrentSet.NewChatterAlert.Instantiate<AlertScript>();
        alert.Text = $"{user.display_name}: {message.Message}";
        _queuedAlerts.Enqueue(alert);
        alert.AlertFinished += PlayNextAlert;
        alert.AlertReady += CheckAlerts;
        EmitSignal(SignalName.ShowAlert, alert);
    }

    private void PlayNextAlert()
    {
        if (_queuedAlerts.Count == 0)
        {
            _currentAlert = null;
            return;
        }
        _currentAlert = _queuedAlerts.Dequeue();
        _currentAlert.StartAnimation();
    }

    private void CheckAlerts(AlertScript alert)
    {
        if (_currentAlert != null) return;
        _currentAlert = _queuedAlerts.Dequeue();
        _currentAlert.StartAnimation();
    }

    private void TwitchManagerOnNewConnection()
    {
        SetupEventSub();
    }
}