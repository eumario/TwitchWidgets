using Godot;
using System.Collections.Generic;
using System.IO;
using Godot.Collections;
using Godot.Sharp.Extras;
using TwitchWidgetsApp.Library;
using AlertSet = TwitchWidgetsApp.Library.AlertSet;
using Globals = TwitchWidgetsApp.Library.Globals;

namespace TwitchWidgetsApp.Scenes;

public partial class Settings : MarginContainer
{
	[Singleton] public Globals? Globals;
	[NodePath] private OptionButton? _sceneCollection;
	[NodePath] private OptionButton? _alertSet;
	[NodePath] private CheckBox? _autoTwitch;
	[NodePath] private CheckBox? _autoObs;
	[NodePath] private CheckBox? _autoPlay;
	[NodePath] private CheckBox? _useSpotify;
	[NodePath] private CheckBox? _useFullScreen;
	[NodePath] private CheckBox? _useScreen;
	[NodePath] private OptionButton? _screenNumber;
	[NodePath] private CheckBox? _debugging;

	[NodePath] private ItemList? _musicFolders;
	[NodePath] private LineEdit? _folderPath;
	[NodePath] private Button? _browseFolder;

	[NodePath] private Button? _addFolder;
	[NodePath] private Button? _saveFolder;
	[NodePath] private Button? _removeFolder;

	[NodePath] private CheckBox? _randomizeMusic;
	[NodePath] private CheckBox? _loopMusic;

	[NodePath] private Button? _saveSettings;
	[NodePath] private Button? _resetSettings;

	private List<SceneCollection> _collections = [];
	private List<AlertSet> _alertSets = [];

	private void LoadSettings()
	{
		LoadSceneCollectionConfig();

		_autoTwitch!.ButtonPressed = Globals!.SettingsManager!.GetValue("auto_connect_twitch", false);
		_autoObs!.ButtonPressed = Globals.SettingsManager.GetValue("auto_connect_obs", false);
		_autoPlay!.ButtonPressed = Globals.SettingsManager.GetValue("auto_play_music", false);
		_useSpotify!.ButtonPressed = Globals.SettingsManager.GetValue("use_spotify", false);
		_useFullScreen!.ButtonPressed = Globals.SettingsManager.GetValue("use_fullscreen", false);
		_useScreen!.ButtonPressed = Globals.SettingsManager.GetValue("use_specific", false);
		_screenNumber!.Disabled = !Globals.SettingsManager.GetValue("use_specific", false);
		_screenNumber.Select(Globals.SettingsManager.GetValue("use_display", 0));
		_randomizeMusic!.ButtonPressed = Globals.SettingsManager.GetValue("randomize_music", false);
		_loopMusic!.ButtonPressed = Globals.SettingsManager.GetValue("loop_music", false);
		_debugging!.ButtonPressed = Globals.SettingsManager.GetValue("is_debugging", false);

		var folders = Globals.SettingsManager.GetValue("music_folders", "");
		if (string.IsNullOrEmpty(folders)) return;

		_musicFolders!.Clear();
		foreach (var folder in Json.ParseString(folders).As<Array<string>>())
			_musicFolders.AddItem(folder);
	}

	private void LoadSceneCollectionConfig()
	{
		var path = Globals!.SettingsManager!.GetValue("scene_collection", "");
		if (path == "")
			_sceneCollection!.Select(-1);
		else
		{
			_sceneCollection!.Select(-1);
			foreach (var (coll, i) in _collections.WithIndex())
			{
				if (coll.ResourcePath != path) continue;
				_sceneCollection.Select(i);
				break;
			}
		}

		path = Globals.SettingsManager.GetValue("alert_set", "");
		if (path == "")
			_alertSet!.Select(-1);
		else
		{
			_alertSet!.Select(-1);
			foreach (var (set, i) in _alertSets.WithIndex())
			{
				if (set.ResourcePath != path) continue;
				_alertSet.Select(i);
				break;
			}
		}
	}

	public override void _Ready()
	{
		this.OnReady();
		var dh = DirAccess.Open("res://Resources/SceneCollection/");
		dh.IncludeHidden = false;
		_collections.Clear();
		_sceneCollection!.Clear();
		GD.Print("Loading scene collections...");
		foreach (var file in dh.GetFiles())
		{
			var actualFile = file;
			if (file.EndsWith(".remap")) actualFile = file.Replace(".remap", "");
			var fullPath = Path.Join("res://Resources/SceneCollection/",actualFile);
			var collection = GD.Load<SceneCollection>(fullPath);
			GD.Print("Loaded collection: File: " + fullPath + " Name: " + collection.CollectionName);
			var nextIndx = _sceneCollection.ItemCount;
			_sceneCollection.AddItem(collection.CollectionName);
			_sceneCollection.SetItemMetadata(nextIndx, collection);
			_collections.Add(collection);
		}
		GD.Print("Finished loading scene collections");
		
		GD.Print("Loading alert sets...");
		dh = DirAccess.Open("res://Resources/AlertCollections/");
		dh.IncludeHidden = false;
		_alertSets.Clear();
		_alertSet!.Clear();
		foreach (var file in dh.GetFiles())
		{
			var actualFile = file;
			if (file.EndsWith(".remap")) actualFile = file.Replace(".remap", "");
			var fullPath = Path.Join("res://Resources/AlertCollections/", actualFile);
			var set = GD.Load<AlertSet>(fullPath);
			GD.Print("Loaded alert set: File: " + fullPath + " Name: " + set.AlertSetName);
			var nextIndx = _alertSet.ItemCount;
			_alertSet.AddItem(set.AlertSetName);
			_alertSet.SetItemMetadata(nextIndx, set);
			_alertSets.Add(set);
		}

		_screenNumber!.Clear();

		for (int i = 0; i < DisplayServer.GetScreenCount(); i++)
		{
			_screenNumber.AddItem($"Screen {i+1}");
		}

		_sceneCollection.Select(-1);
		_sceneCollection.ItemSelected += index =>
		{
			var collection = _sceneCollection.GetItemMetadata((int)index).As<SceneCollection>();
			Globals!.ProjectionWindow!.CurrentCollection = collection;
			Globals!.SettingsManager!.SetValue("scene_collection", collection.ResourcePath);
			GD.Print(collection.ResourcePath);
		};
		
		_alertSet.Select(-1);
		_alertSet.ItemSelected += index =>
		{
			var set = _alertSet.GetItemMetadata((int)index).As<AlertSet>();
			Globals!.AlertManager!.CurrentSet = set;
			Globals!.SettingsManager!.SetValue("alert_set", set.ResourcePath);
			GD.Print(set.ResourcePath);
		};

		_useScreen!.Pressed += () => _screenNumber.Disabled = !_useScreen.ButtonPressed;

		_browseFolder!.Pressed += () =>
		{
			DisplayServer.FileDialogShow("Add Music Folder", OS.GetSystemDir(OS.SystemDir.Music), "", false,
				DisplayServer.FileDialogMode.OpenDir, [], Callable.From<bool, string[], int>(HandleBrowseFolder));
		};

		_addFolder!.Pressed += () =>
		{
			if (string.IsNullOrEmpty(_folderPath!.Text)) return;
			_musicFolders!.AddItem(_folderPath.Text);
			_folderPath.Text = "";
		};

		_saveFolder!.Pressed += () =>
		{
			if (string.IsNullOrEmpty(_folderPath!.Text)) return;
			if (_musicFolders!.GetSelectedItems().Length == 0) return;

			var indx = _musicFolders.GetSelectedItems()[0];
			_musicFolders.SetItemText(indx, _folderPath.Text);
			_folderPath.Text = "";
		};

		_removeFolder!.Pressed += () =>
		{
			if (_musicFolders!.GetSelectedItems().Length == 0) return;

			var indx = _musicFolders.GetSelectedItems()[0];
			_musicFolders.RemoveItem(indx);
		};

		_saveSettings!.Pressed += UpdateAndSaveSettings;

		_resetSettings!.Pressed += () =>
		{
			Globals!.SettingsManager!.ResetSettings();
			LoadSettings();
		};

		LoadSettings();
	}

	private void UpdateAndSaveSettings()
	{
		Globals!.SettingsManager!.SetValue("auto_connect_twitch", _autoTwitch!.ButtonPressed);
		Globals.SettingsManager.SetValue("auto_connect_obs", _autoObs!.ButtonPressed);
		Globals.SettingsManager.SetValue("auto_play_music", _autoPlay!.ButtonPressed);
		Globals.SettingsManager.SetValue("use_spotify", _useSpotify!.ButtonPressed);
		Globals.SettingsManager.SetValue("use_fullscreen", _useFullScreen!.ButtonPressed);
		Globals.SettingsManager.SetValue("use_specific", _useScreen!.ButtonPressed);
		Globals.SettingsManager.SetValue("use_display", _screenNumber!.Selected);
		Globals.SettingsManager.SetValue("randomize_music", _randomizeMusic!.ButtonPressed);
		Globals.SettingsManager.SetValue("loop_music", _loopMusic!.ButtonPressed);
		Globals.SettingsManager.SetValue("is_debugging", _debugging!.ButtonPressed);
		
		var folders = new Array<string>();
		for (var i = 0; i < _musicFolders!.ItemCount; i++)
		{
			var item = _musicFolders.GetItemText(i);
			folders.Add(item);
		}
		
		Globals.SettingsManager.SetValue("music_folders",Json.Stringify(folders));
		Globals.SettingsManager.SaveSettings();
		Globals.EmitSignal(Globals.SignalName.SettingsUpdated);
		OS.Alert("Settings have been saved");
	}

	private void HandleBrowseFolder(bool status, string[] paths, int filterIndex)
	{
		if (paths.Length >= 1)
			_folderPath!.Text = paths[0];
	}
}
