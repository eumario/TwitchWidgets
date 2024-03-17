using Godot;
using System.Linq;
using Godot.Sharp.Extras;

namespace TwitchWidgetsApp.Library.Controls;

public partial class AccountInfo : GridContainer
{
	[Singleton] public Globals Globals;
	[NodePath] private Label _streamerName;
	[NodePath] private TextureRect _streamerPfp;
	[NodePath] private Panel _streamerOnline;
	[NodePath] private Label _botName;
	[NodePath] private TextureRect _botPfp;
	[NodePath] private Panel _botOnline;

	[NodePath] private Button _connectTwitch;
	[NodePath] private Button _disconnectTwitch;
	public override void _Ready()
	{
		this.OnReady();
		
		UpdateInfo();
		_connectTwitch.Pressed += ConnectTwitchOnPressed;
		_disconnectTwitch.Pressed += DisconnectTwitchOnPressed;
		MonitorConnection();
	}

	private void ConnectTwitchOnPressed()
	{
		// Handle Connection
		Globals.TwitchManager.ConnectTwitch();
	}

	private void MonitorConnection()
	{
		if (Globals.TwitchManager == null) { Globals.RunOnMain(MonitorConnection); return; }
		if (Globals.Chat == null) { Globals.RunOnMain(MonitorConnection); return; }
		if (Globals.TwitchManager.EventSub == null) { Globals.RunOnMain(MonitorConnection); return; }
		_streamerOnline.SelfModulate = Globals.TwitchManager.IsEventSubConnected ? Colors.Green : Colors.Red;
		_botOnline.SelfModulate = Globals.TwitchManager.IsChatConnected ? Colors.Green : Colors.Red;
		GetTree().CreateTimer(1).Timeout += MonitorConnection;
	}

	private void DisconnectTwitchOnPressed()
	{
		// Handle Disconnection
		Globals.Chat.SendMessage(Globals.Streamer, "Disconnecting.");
		Globals.TwitchManager.DisconnectTwitch();
	}

	private async void UpdateInfo()
	{
		if (Globals.Database?.Secrets == null) { Globals.RunOnMain(UpdateInfo); return; }

		var res = Globals.Database.Secrets.ToList();
		if (res.Count == 0) return;

		if (!string.IsNullOrEmpty(res[0].StreamerDisplayName)) _streamerName.Text = res[0].StreamerDisplayName;
		if (!string.IsNullOrEmpty(res[0].StreamerProfilePic)) _streamerPfp.Texture = await Util.FetchImage(res[0].StreamerProfilePic);
		// Check if Streamer is connected

		if (!string.IsNullOrEmpty(res[0].BotDisplayName)) _botName.Text = res[0].BotDisplayName;
		if (!string.IsNullOrEmpty(res[0].BotProfilePic)) _botPfp.Texture = await Util.FetchImage(res[0].BotProfilePic);
		// Check if Bot is connected
	}
}
