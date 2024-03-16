using Godot;
using Godot.Sharp.Extras;
using OBSStudioClient.Events;
using TwitchWidgets.Data.Context;
using TwitchWidgetsApp.Library;
using Globals = TwitchWidgetsApp.Library.Globals;
using ObsScene = TwitchWidgetsApp.Library.Controls.ObsScene;

namespace TwitchWidgetsApp.Scenes;

public partial class ProjectionWindow : Control
{
	[Singleton] public Globals Globals;

	[Singleton] public Node ElgatoStreamDeck;

	private SceneCollection _currentCollection;

	[Export]
	public SceneCollection CurrentCollection
	{
		get => _currentCollection;
		set
		{
			_currentCollection = value;
			if (Globals.ObsManager.IsConnected())
				FetchScene();
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		GetTree().Root.Transparent = true;
		GetTree().Root.TransparentBg = true;
		GetViewport().TransparentBg = true;
		TwitchWidgetsContext.DatabaseLocation = System.IO.Path.Join(OS.GetUserDataDir(), "twitch_widgets.db");
		Globals.Database = new TwitchWidgetsContext();
		Globals.SettingsManager.LoadSettings();
		Globals.ProjectorWindow = GetWindow();
		Globals.ProjectionWindow = this;
		GetWindow().Title = "TwitchWidgets";
		
		Globals.SettingsUpdated += GlobalsOnSettingsUpdated;
		
		ElgatoStreamDeck.Connect("on_key_down", Callable.From((string data) => HandleStreamDeck(data)));

		Globals.ObsManager.ObsConnected += () =>
		{
			Globals.ObsManager.Client.CurrentProgramSceneChanged += ClientOnCurrentProgramSceneChanged;

			FetchScene();
		};
		
		GlobalsOnSettingsUpdated();
		
		Globals.EmitSignal(Globals.SignalName.SettingsLoaded);

		if (!Globals.ObsManager.IsConnected()) return;
		
		Globals.ObsManager.Client.CurrentProgramSceneChanged += ClientOnCurrentProgramSceneChanged;
		FetchScene();
	}

	private async void FetchScene()
	{
		if (!Globals.ObsManager.IsConnected())
		{
			GD.Print("FetchScene called with no OBS Connection!");
			return;
		}
		var scene = await Globals.ObsManager.Client.GetCurrentProgramScene();
		ChangeObsScene(scene);
	}

	private void ClientOnCurrentProgramSceneChanged(object sender, SceneNameEventArgs e)
	{
		if (CurrentCollection == null) return;
		Globals.RunOnMain(() =>
		{
			ChangeObsScene(e.SceneName);
		});
	}

	private void ChangeObsScene(string sceneName)
	{
		if (CurrentCollection == null) { Globals.RunOnMain(() => ChangeObsScene(sceneName)); return; }
		var ps = CurrentCollection[sceneName];
		
		foreach (var node in GetChildren())
		{
			if (node is ObsScene)
				node.QueueFree();
		}
		AddChild(ps.Instantiate());
	}
	
	private void GlobalsOnSettingsUpdated()
	{
		var path = Globals.SettingsManager.GetValue("scene_collection", "");
		if (path != "")
		{
			if (CurrentCollection == null || CurrentCollection.ResourcePath != path)
				CurrentCollection = GD.Load<SceneCollection>(path);
		}

		if (Globals.UseSpecificScreen)
		{
			DisplayServer.WindowSetCurrentScreen(Globals.SpecificScreen);
		}

		GetWindow().Mode = Globals.UseFullscreen ? Window.ModeEnum.Fullscreen : Window.ModeEnum.Windowed;
	}

	private void HandleStreamDeck(string msg)
	{
		if (msg.StartsWith("scene"))
		{
			var scene = msg.Split(" ")[1];
			var ps = GD.Load<PackedScene>(scene);
			var node = ps.Instantiate();
			foreach (var child in GetChildren())
				child.QueueFree();
			AddChild(node);
		}
	}
}
