using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Godot.Sharp.Extras;
using Twitch.Base.Models.Clients.Chat;
using Twitch.Base.Models.NewAPI.Users;
using TwitchWidgets.Data.Context;
using TwitchWidgets.Data.Models;
using TwitchWidgetsApp.Library.Managers;
using TwitchWidgetsApp.Scenes;
using TwitchWidgetsApp.Scenes.CatCafe.Scenes;

namespace TwitchWidgetsApp.Library;

public partial class Globals : Node
{
    #region Global Signals
    [Signal] public delegate void CredentialsUpdatedEventHandler();
    [Signal] public delegate void SettingsLoadedEventHandler();
    [Signal] public delegate void SettingsUpdatedEventHandler();
    [Signal] public delegate void UpdateSkinEventHandler(string userId, string skin);
    [Signal] public delegate void UpdateCommandsEventHandler();
    [Signal] public delegate void UpdateTickerEventHandler();
    #endregion
    
    #region Global Variables
    [Singleton] public Node ElgatoStreamDeck;
    public TwitchWidgetsContext Database;
    [Resource("res://Scenes/SettingsWindow.tscn")] private PackedScene _settings;

    public ObsManager ObsManager { get; set; }
    public TwitchManager TwitchManager { get; set; }
    public SettingsManager SettingsManager { get; set; }
    public ImageManager ImageManager { get; set; }
    public MusicController MusicController { get; set; }
    public TtsManager TtsManager { get; set; }
    public AlertManager AlertManager { get; set; }
    
    public BotManager BotManager { get; set; }

    public List<ChatMessagePacketModel> ChatHistory { get; set; } = [];
    
    public System.Collections.Generic.Dictionary<UserModel, Color> SavedChatColors = new();

    public List<UserModel> Chatters { get; set; } = [];
    
    public Window ProjectorWindow { get; set; }
    public ProjectionWindow ProjectionWindow { get; set; }
    public Window SettingsWindow { get; set; }

    public List<StreamAvatar> StreamAvatars = [];
    public List<KnownChatter> KnownChatters = [];

    #endregion
    
    #region Global Settings Variables (Read Only)
    public bool IsDebugging => SettingsManager.GetValue("is_debugging", false);
    public bool AutoConnectTwitch => SettingsManager.GetValue("auto_connect_twitch", false);
    public bool AutoConnectObs => SettingsManager.GetValue("auto_connect_obs", false);
    public bool AutoPlayMusic => SettingsManager.GetValue("auto_play_music", false);
    public bool UseSpotify => SettingsManager.GetValue("use_spotify", false);
    public bool UseFullscreen => SettingsManager.GetValue("use_fullscreen", false);
    public bool UseSpecificScreen => SettingsManager.GetValue("use_specific", false);
    public int SpecificScreen => SettingsManager.GetValue("use_display", 0);

    public string ObsHost => SettingsManager.GetValue("obs_host", "");
    public int ObsPort => SettingsManager.GetValue("obs_port", 0);
    public string ObsPass => SettingsManager.GetValue("obs_pass", "");
    public bool RandomizeMusic => SettingsManager.GetValue("randomize_music", false);
    public bool LoopMusic => SettingsManager.GetValue("loop_music", false);
    public bool UseMockServer => SettingsManager.GetValue("use_mock_server", false);

    public List<string> MusicFolders =>
        Json.ParseString(SettingsManager.GetValue("music_folders", "")).As<Array<string>>().ToList();
    #endregion
    
    #region Private Variables
    private List<Action> _mainThreadActions = [];
    #endregion

    #region Godot Overrides

    public override void _Ready()
    {
        this.OnReady();
        ObsManager = new ObsManager();
        TwitchManager = new TwitchManager();
        SettingsManager = new SettingsManager();
        ImageManager = new ImageManager();
        MusicController = new MusicController();
        TtsManager = new TtsManager();
        AlertManager = new AlertManager();
        BotManager = new BotManager();
        ObsManager.Name = "ObsManager";
        TwitchManager.Name = "TwitchManager";
        SettingsManager.Name = "SettingsManager";
        ImageManager.Name = "ImageManager";
        MusicController.Name = "MusicController";
        TtsManager.Name = "TtsManager";
        AlertManager.Name = "AlertManager";
        BotManager.Name = "BotManager";
        AddChild(ObsManager);
        AddChild(TwitchManager);
        AddChild(SettingsManager);
        AddChild(ImageManager);
        AddChild(MusicController);
        AddChild(TtsManager);
        AddChild(AlertManager);
        AddChild(BotManager);
        ElgatoStreamDeck.Connect("on_key_down", Callable.From((string data) => ElgatoCall(data)));
    }

    private void ProcessOnMainThread()
    {
        if (_mainThreadActions.Count == 0) return;
        var finished = new List<Action>();
        var iterate = new List<Action>(_mainThreadActions);
        foreach (var action in iterate)
        {
            action.Invoke();
            finished.Add(action);
        }

        foreach (var action in finished) _mainThreadActions.Remove(action);
    }

    public void RunOnMain(Action action) => _mainThreadActions.Add(action);

    public override void _Process(double delta)
    {
        ProcessOnMainThread();
        if (Input.IsActionJustPressed("fullscreen"))
        {
            ProjectorWindow.Mode = ProjectorWindow.Mode == Window.ModeEnum.Fullscreen
                ? Window.ModeEnum.Maximized
                : Window.ModeEnum.Fullscreen;
        }

        if (Input.IsActionJustPressed("show_settings"))
        {
            ShowSettingsWindow();
        }
    }

    private void ShowSettingsWindow()
    {
        if (SettingsWindow != null) return;
            
        var window = new Window();
        window.Title = "TwitchWidgets Settings";
        SettingsWindow = window;
        GetTree().Root.AddChild(window);
        window.PopupCentered(new Vector2I(1152, 688));
        var panel = _settings.Instantiate<SettingsWindow>();
        window.AddChild(panel);
        panel.Position = Vector2.Zero;
        window.CloseRequested += () =>
        {
            window.QueueFree();
            SettingsWindow = null;
        };
    }

    private void ElgatoCall(string message)
    {
        switch (message)
        {
            case "settings":
                ShowSettingsWindow();
                break;
            case "toggle-fullscreen":
                ProjectorWindow.Mode = ProjectorWindow.Mode == Window.ModeEnum.Fullscreen
                    ? Window.ModeEnum.Maximized
                    : Window.ModeEnum.Fullscreen;
                break;
            case "shutdown overlay":
                GetTree().Quit();
                break;
        }
    }

    #endregion
}