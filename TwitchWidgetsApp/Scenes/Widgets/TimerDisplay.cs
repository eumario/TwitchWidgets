using Godot;
using System;

namespace TwitchWidgetsApp.Scenes.Widgets;

[Tool]
[GlobalClass]
public partial class TimerDisplay : Label
{
	[Export] public float TotalTime = 60;
	[Export] public string TimeUpMessage = "Starting soon...";
	[Export] public bool Countdown = true;

	[Export]
	public LabelSettings FontSettings
	{
		get => LabelSettings;
		set => LabelSettings = value;
	}
	private Timer _timer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = new Timer();
		_timer.Autostart = true;
		_timer.OneShot = false;
		_timer.WaitTime = 1.0;
		AddChild(_timer);
		if (!Countdown) TotalTime = 0;
		_timer.Timeout += () =>
		{
			TotalTime += Countdown ? -1 : 1;
			UpdateText();
		};
		UpdateText();
	}

	private void UpdateText()
	{
		var ts = TimeSpan.FromSeconds(TotalTime);
		Text = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
		if (Countdown && TotalTime <= 0)
		{
			Text = TimeUpMessage;
			_timer.Stop();
		}
	}
}
