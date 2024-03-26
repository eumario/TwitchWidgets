using Godot;
using System.Collections.Generic;
using Godot.Sharp.Extras;
using OBSStudioClient.Classes;
using TwitchWidgetsApp.Resources.AlertCollections.ForestAlerts;
using Globals = TwitchWidgetsApp.Library.Globals;

namespace TwitchWidgetsApp.Resources.SceneCollection.ForestTheme;

public partial class DodgeAlerts : Node
{
	[Singleton] public Globals? Globals;

	[Export] public string? SceneName;
	[Export] public string? SourceToDodge;
	[Export] public Vector2 TargetPosition;
	[Export] public Vector2 TargetSize;
	private SceneItem? _sourceToDodge;

	private Rect2 _originTransform;
	private Rect2 _newTransform;
	private List<AlertScript> _alerts = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		GatherInfo();
		Globals!.AlertManager!.ShowAlert += HandleAlert;
	}

	public override void _ExitTree()
	{
		Globals!.AlertManager!.ShowAlert -= HandleAlert;
	}

	private void HandleAlert(AlertScript alert)
	{
		if (_alerts.Count == 0)
			StartTransition();
		_alerts.Add(alert);
		alert.AlertFinished += () =>
		{
			_alerts.Remove(alert);
			if (_alerts.Count == 0)
				EndTransition();
		};
	}

	private async void GatherInfo()
	{
		var list = await Globals!.ObsManager!.Client!.GetSceneItemList(SceneName!);
		foreach (var item in list)
		{
			if (item.SourceName == SourceToDodge)
				_sourceToDodge = item;
		}

		if (_sourceToDodge == null)
		{
			GD.PrintErr("Source to Dodge is not found in OBS scene.");
			return;
		}

		var transform = _sourceToDodge.SceneItemTransform;

		_originTransform = new Rect2();
		_originTransform.Position = new Vector2(transform.PositionX, transform.PositionY);
		_originTransform.Size = new Vector2(transform.ScaleX, transform.ScaleY);

		_newTransform = new Rect2();
		_newTransform.Position = TargetPosition;
		_newTransform.Size = TargetSize;
	}

	private void StartTransition()
	{
		if (_sourceToDodge == null) return;

		var tween = CreateTween();
		tween.TweenMethod(Callable.From<Rect2>(AdjustTransform), _originTransform, _newTransform, 0.3f);
	}

	private void EndTransition()
	{
		var tween = CreateTween();
		tween.TweenMethod(Callable.From<Rect2>(AdjustTransform), _newTransform, _originTransform, 0.3f);
	}

	private async void AdjustTransform(Rect2 newPos)
	{
		var oldTransform = _sourceToDodge!.SceneItemTransform;
		var newTransform = new SceneItemTransform(oldTransform.Alignment,
			oldTransform.BoundsAlignment,
			1,
			oldTransform.BoundsType,
			1,
			oldTransform.CropBottom,
			oldTransform.CropLeft,
			oldTransform.CropRight,
			oldTransform.CropTop,
			newPos.Size.Y,
			newPos.Position.X,
			newPos.Position.Y,
			oldTransform.Rotation,
			newPos.Size.X,
			newPos.Size.Y,
			oldTransform.SourceHeight,
			oldTransform.SourceWidth,
			newPos.Size.X);
		await Globals!.ObsManager!.Client!.SetSceneItemTransform(SceneName!, _sourceToDodge.SceneItemId, newTransform);
	}
}
