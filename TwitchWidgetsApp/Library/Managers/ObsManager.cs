using Godot;
using Godot.Sharp.Extras;
using OBSStudioClient;

namespace TwitchWidgetsApp.Library.Managers;

public partial class ObsManager : Node
{
    [Singleton] public Globals Globals;
    [Signal] public delegate void ObsConnectedEventHandler();
    public ObsClient Client;

    public override void _Ready()
    {
        this.OnReady();
        Globals.SettingsLoaded += async () =>
        {
            if (!Globals.AutoConnectObs) return;
            var client = new ObsClient();
            var res = await client.ConnectAsync(true, Globals.ObsPass, Globals.ObsHost, Globals.ObsPort);
            if (res)
                SetConnection(client);
            else
            {
                Client = null;
                OS.Alert("Failed to connect to OBS.");
            }
        };
    }

    public void SetConnection(ObsClient client)
    {
        Client = client;
        SetupEvents();
    }

    private void SetupEvents()
    {
        Client.ConnectionClosed += (sender, args) => Client = null;
        EmitSignal(SignalName.ObsConnected);
    }

    public bool IsConnected() => Client != null;
}