using Godot;
using Godot.Sharp.Extras;
using TwitchWidgetsApp.Library;

namespace TwitchWidgetsApp.Scenes;

public partial class Text_To_Speech : MarginContainer
{
	[Singleton] public Globals Globals;

	[NodePath] private ItemList _voiceList;
	[NodePath] private LineEdit _testMessage;
	[NodePath] private Button _testSpeak;
	private TreeItem _root;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		var savedVoice = Globals.SettingsManager.GetValue("tts_voice","");
		var savedIndex = -1;
		foreach (var (voice, indx) in Globals.TtsManager.Voices.WithIndex())
		{
			_voiceList.AddItem(voice);
			if (voice != savedVoice) continue;
			savedIndex = indx;
			Globals.TtsManager.SelectedVoice = voice;
		}
		
		if (savedIndex >= 0)
			_voiceList.Select(savedIndex);
		
		Globals.TtsManager.TtsDownloaded += TtsManagerOnTtsDownloaded;
		Globals.TtsManager.TtsFinished += TtsManagerOnTtsFinished;
		
		_voiceList.ItemSelected += index =>
		{
			var voice = _voiceList.GetItemText((int)index);
			Globals.SettingsManager.SetValue("tts_voice", voice);
			Globals.SettingsManager.SaveSettings();
			Globals.TtsManager.SelectedVoice = voice;
		};

		_testSpeak.Pressed += () =>
		{
			_testMessage.Editable = false;
			_testSpeak.Disabled = true;
			Globals.TtsManager.AddTtsMessage(_testMessage.Text);
		};
	}

	public override void _ExitTree()
	{
		Globals.TtsManager.TtsDownloaded -= TtsManagerOnTtsDownloaded;
		Globals.TtsManager.TtsFinished -= TtsManagerOnTtsFinished;
	}
	
	private void TtsManagerOnTtsFinished(string msg)
	{
		Globals.TtsManager.ClearAll();
		_testMessage.Editable = true;
		_testSpeak.Disabled = false;
	}

	private void TtsManagerOnTtsDownloaded(string msg)
	{
		Globals.TtsManager.PlayNextMessage();
	}
}
