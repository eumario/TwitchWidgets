using Godot;
using Godot.Sharp.Extras;
using Globals = TwitchWidgetsApp.Library.Globals;

namespace TwitchWidgetsApp.Resources.AlertCollections.ForestAlerts;

public partial class AlertScript : Control
{
	[Singleton] public Globals? Globals;

	[Signal] public delegate void AlertReadyEventHandler(AlertScript alert);
	[Signal] public delegate void AlertFinishedEventHandler();

	[NodePath] private TextureRect? _banner;
	[NodePath] private TextureRect? _logo;
	[NodePath] private Label? _text;

	private string? _logoPath;
	private string? _textString;

	[Export] public AnimationMode Animation;

	[Export(PropertyHint.File)] public string? Logo
	{
		get => _logoPath;
		set
		{
			_logoPath = value;
			if (value == null) return;
			if (_logo != null)
				_logo.Texture = GD.Load<Texture2D>(_logoPath);
		}
	}

	public string? Text
	{
		get => _textString;
		set
		{
			_textString = value;
			if (_text != null)
				_text.Text = _textString;
		}
	}
	
	public override void _Ready()
	{
		this.OnReady();
		ResetAnimation();
		Logo = _logoPath;
		Text = _textString;
		EmitSignal(SignalName.AlertReady, this);
	}

	public void ResetAnimation()
	{
		switch (Animation)
		{
			case AnimationMode.SlideFade:
				_banner!.Position = new Vector2(_banner.Position.X, -Size.Y);
				_banner.Modulate = Colors.Transparent;
				_text!.VisibleRatio = 0.0f;
				break;
			case AnimationMode.Rollout:
				// 739.5 -> 218
				_text!.Size = new Vector2(0, 195);
				_text.Position = new Vector2(739.5f, 0);
				_text.VisibleRatio = 0.0f;
				_text.Modulate = Colors.Transparent;
				break;
		}
	}

	public void StartAnimation()
	{
		if (Animation == AnimationMode.SlideFade) AnimateSlideFade();
		if (Animation == AnimationMode.Rollout) AnimateRollout();
	}

	private void AnimateSlideFade()
	{
		var tween = CreateTween().SetParallel(true);
		tween.TweenProperty(_banner, "position", new Vector2(_banner!.Position.X, 0), 1.0f);
		tween.TweenProperty(_banner, "modulate", Colors.White, 1.75f);
		tween.Chain().TweenProperty(_text, "visible_ratio", 1.0f, 1.5f);
		tween.Chain().TweenInterval(3.0);
		tween.Chain().TweenProperty(_text, "visible_ratio", 0.0f, 1.5f);
		tween.Chain().TweenProperty(_banner, "modulate", Colors.Transparent, 1.5f);
		tween.TweenProperty(_banner, "position", new Vector2(_banner.Position.X, -Size.Y), 1.5f);
		tween.Chain().TweenCallback(Callable.From(() => EmitSignal(SignalName.AlertFinished)));
		tween.Chain().TweenCallback(Callable.From(QueueFree));
	}

	private void AnimateRollout()
	{
		var tween = CreateTween().SetParallel(true);
		tween.TweenProperty(_text, "size", new Vector2(1484, 195), 1.0f);
		tween.TweenProperty(_text, "position", new Vector2(218, 0), 1.0f);
		tween.TweenProperty(_text, "modulate", Colors.White, 0.65f);
		tween.Chain().TweenProperty(_text, "visible_ratio", 1.0f, 1.5f);
		tween.Chain().TweenInterval(5.0);
		tween.Chain().TweenProperty(_text, "modulate", Colors.Transparent, 1.5f);
		tween.Chain().TweenCallback(Callable.From(() => EmitSignal(SignalName.AlertFinished)));
		tween.Chain().TweenCallback(Callable.From(QueueFree));
	}

	public enum AnimationMode
	{
		SlideFade,
		Rollout
	}
}
