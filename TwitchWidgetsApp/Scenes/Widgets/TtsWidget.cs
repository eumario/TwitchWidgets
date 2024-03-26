using Godot;
using Godot.Sharp.Extras;
using TwitchWidgetsApp.Library;

public partial class TtsWidget : Control
{
	[Singleton] public Globals? Globals;
	[Singleton] public Node? ElgatoStreamDeck;

	[NodePath] private Label? _queueCount;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		ElgatoStreamDeck!.Connect("on_key_down", Callable.From((string data) => HandleStreamDeck(data)));
	}

	private void HandleStreamDeck(string data)
	{
		switch (data)
		{
			case "tts play_previous":
				Globals!.TtsManager!.PlayPreviousMessage();
				break;
			case "tts play_next":
				Globals!.TtsManager!.PlayNextMessage();
				break;
			case "tts repeat":
				Globals!.TtsManager!.RepeatLastMessage();
				break;
			case "tts stop":
				Globals!.TtsManager!.StopMessage();
				break;
			case "tts skip":
				Globals!.TtsManager!.SkipMessage();
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Globals?.TtsManager == null) return;
		_queueCount!.Text = $"{Globals.TtsManager.CurrentQueue}/{Globals.TtsManager.TotalQueue}";
	}
}
