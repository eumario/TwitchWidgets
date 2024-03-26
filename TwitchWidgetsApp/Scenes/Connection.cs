using Godot;
using Godot.Sharp.Extras;
using OBSStudioClient;
using TwitchWidgetsApp.Library.Controls;
using Globals = TwitchWidgetsApp.Library.Globals;

namespace TwitchWidgetsApp.Scenes;

public partial class Connection : MarginContainer
{
	[Singleton] public Globals? Globals;
	[NodePath] private ObsSettings? _obsSettings;
	[NodePath] private Button? _connectObs;
	[NodePath] private CheckBox? _useMockServer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		_connectObs!.Pressed += ConnectObsOnPressed;
		if (Globals!.SettingsManager == null) { Globals.RunOnMain(LoadSettings); return; }

		_useMockServer!.Toggled += async (val) =>
		{
			GD.Print($"Use Mock Server: {val}");
			Globals.SettingsManager.SetValue("use_mock_server", val);
			await Globals!.Database!.SaveChangesAsync();
			Globals.EmitSignal(Globals.SignalName.SettingsUpdated);
		};

		LoadSettings();
	}

	private void LoadSettings()
	{
		_obsSettings!.Host = Globals!.SettingsManager!.GetValue("obs_host", "localhost");
		_obsSettings.Port = Globals.SettingsManager.GetValue("obs_port", 4455).ToString();
		_obsSettings.Password = Globals.SettingsManager.GetValue("obs_pass", "");
		_useMockServer!.ButtonPressed = Globals.UseMockServer;
	}

	private async void ConnectObsOnPressed()
	{
		if (Globals!.ObsManager!.IsConnected())
		{
			OS.Alert("You are already connected to OBS Studio.");
			return;
		}
		var host = _obsSettings!.Host;
		var port = _obsSettings.Port;
		var pass = _obsSettings.Password;
		var client = new ObsClient();
		client.AutoReconnect = true;
		var res = await client.ConnectAsync(true, pass, host, int.Parse(port));
		if (!res)
		{
			OS.Alert("Failed to connect to OBS.");
			return;
		}
		
		Globals!.SettingsManager!.SetValue("obs_host", host);
		Globals.SettingsManager.SetValue("obs_port", int.Parse(port));
		Globals.SettingsManager.SetValue("obs_pass", pass);
		Globals.SettingsManager.SaveSettings();
		
		Globals.ObsManager.SetConnection(client);
	}
}
