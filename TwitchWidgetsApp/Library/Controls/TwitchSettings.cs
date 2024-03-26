using System.Linq;
using Godot;
using System.Threading.Tasks;
using Godot.Sharp.Extras;
using Twitch.Base;
using TwitchWidgets.Data.Models;

namespace TwitchWidgetsApp.Library.Controls;
public partial class TwitchSettings : GridContainer
{
	[Singleton] public Globals? Globals;
	[NodePath] private LineEdit? _clientId;
	[NodePath] private LineEdit? _clientSecret;
	[NodePath] private Button? _authStreamer;
	[NodePath] private Button? _authBot;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		_authStreamer!.Pressed += AuthStreamerOnPressed;
		_authBot!.Pressed += AuthBotOnPressed;

		_clientId!.FocusEntered += () => _clientId.Secret = false;
		_clientId.FocusExited += () => _clientId.Secret = true;
		_clientSecret!.FocusEntered += () => _clientSecret.Secret = false;
		_clientSecret.FocusExited += () => _clientId.Secret = true;

		UpdateInfo();
	}

	private void UpdateInfo()
	{
		if (Globals!.Database?.Secrets == null) { Globals.RunOnMain(UpdateInfo); return; }

		var res = Globals.Database.Secrets.ToList();
		if (res.Count == 0) return;

		_clientId!.Text = res[0].ClientId;
		_clientSecret!.Text = res[0].ClientSecret;
	}

	private async Task<bool> AuthenticateUser(bool isBot = false)
	{
		var clientId = _clientId!.Text;
		var clientSecret = _clientSecret!.Text;

		if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
		{
			OS.Alert("You need to provide a Client ID and Client Secret to authorize TwitchWidgets.");
			return false;
		}

		var conn = await TwitchConnection.ConnectViaLocalhostOAuthBrowser(clientId, clientSecret, TwitchScopes.Scopes, forceApprovalPrompt: true);
		if (conn == null)
		{
			OS.Alert($"Failed to Authenticate " + (isBot ? "Bot" : "Streamer") + " with ClientID and Client Secret provided.");
			return false;
		}

		var tokens = conn.GetOAuthTokenCopy();

		var user = await conn.NewAPI.Users.GetCurrentUser();

		if (user == null)
		{
			OS.Alert("User is not valid, or unable to fetch user data!");
			return false;
		}

		var res = Globals!.Database!.Secrets!.ToList().FirstOrDefault() ?? new Secret();

		res.ClientId = clientId;
		res.ClientSecret = clientSecret;

		if (isBot)
		{
			res.BotAuthToken = tokens.accessToken;
			res.BotRefreshToken = tokens.refreshToken;
			res.BotUserId = user.id;
			res.BotUserName = user.login;
			res.BotDisplayName = user.display_name;
			res.BotProfilePic = user.profile_image_url;
		}
		else
		{
			res.StreamerAuthToken = tokens.accessToken;
			res.StreamerRefreshToken = tokens.refreshToken;
			res.StreamerUserId = user.id;
			res.StreamerUserName = user.login;
			res.StreamerDisplayName = user.display_name;
			res.StreamerProfilePic = user.profile_image_url;
		}

		if (res == null || res.Id == 0)
			Globals!.Database.Secrets!.Add(res!);
		Globals.Database.SaveChanges();

		if (!string.IsNullOrEmpty(res!.StreamerAuthToken) && !string.IsNullOrEmpty(res.BotAuthToken))
			Globals.EmitSignal(Globals.SignalName.CredentialsUpdated);
		
		return true;
	}
	
	private async void AuthStreamerOnPressed()
	{
		if (await AuthenticateUser()) OS.Alert("Successfully Authenticated Streamer Account.");
	}
	
	private async void AuthBotOnPressed()
	{
		if (await AuthenticateUser(true)) OS.Alert("Successfully Authenticated Bot Account.");
	}
}
