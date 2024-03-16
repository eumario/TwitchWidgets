using Godot;
using System.Collections.Generic;
using System.Linq;
using Godot.Sharp.Extras;
using Twitch.Base.Models.NewAPI.Users;
using Globals = TwitchWidgetsApp.Library.Globals;

namespace TwitchWidgetsApp.Scenes.CatCafe.Scenes;

public partial class Chatter : Node2D
{
	[Singleton] public Globals Globals;
	[Signal] public delegate void RandomizeSkinEventHandler();
	#region Paths and Exports
	[NodePath] private Sprite2D _sprite;
	[NodePath] private AnimationPlayer _animPlayer;
	[NodePath] private AnimationTree _animTree;
	[NodePath] private Label _chatterName;
	[NodePath] private Sprite2D _destMarker;
	[NodePath] private Label _currentState;
	[Export] public TileMap Map;
	#endregion
	
	#region Constants

	public List<CurrentState> AvailableStates =
	[
		CurrentState.Walking, CurrentState.Running, CurrentState.SittingDown, CurrentState.LayingDown,
		CurrentState.LookingAround
	];
	#endregion
	
	[ExportCategory("Debug Information")]
	#region Private Variables
	private AStarGrid2D _grid;
	private Texture2D _avatarSkin;
	[Export] private Color _chatterColor;
	[Export(PropertyHint.Enum)] private CurrentState _state;
	[Export] private bool _isMoving;
	[Export] private double _delay = 0.0;
	private RandomNumberGenerator _rng;
	[Export] private Vector2 _randomSpot = Vector2.Zero;
	private Node2D _debugOverlay;
	#endregion
	
	#region Public Properties
	public Color ChatterColor
	{
		get => _chatterColor;
		set
		{
			_chatterColor = value;
			if (_chatterName != null) _chatterName.LabelSettings.FontColor = value;
		}
	}

	private string _chatterDisplayName;
	public string ChatterName
	{
		get => _chatterDisplayName;
		set
		{
			_chatterDisplayName = value;
			if (_chatterName != null) _chatterName.Text = value;
		}
	}

	public Texture2D AvatarSkin
	{
		get => _avatarSkin;
		set
		{
			_avatarSkin = value;
			if (_sprite != null) _sprite.Texture = value;
		}
	}

	public UserModel UserModel { get; set; }
	#endregion

	public static Chatter Create()
	{
		var resource = GD.Load<PackedScene>("res://Scenes/CatCafe/Scenes/Chatter.tscn");
		return resource.Instantiate<Chatter>();
	}

	public StreamAvatar SnapCurrentState()
	{
		return new StreamAvatar()
		{
			UserModel = UserModel,
			State = _state,
			TargetPosition = GlobalPosition,
			SpritePosition = _sprite.GlobalPosition,
			AvatarSkin = _avatarSkin,
			Delay = _delay
		};
	}

	public void RestoreState(StreamAvatar avatar)
	{
		if (!IsInsideTree())
		{
			GD.PrintErr("Unable to restore state for Avatar, till it has been added to the tree.");
			return;
		}
		
		_state = avatar.State;
		UserModel = avatar.UserModel;
		GlobalPosition = avatar.TargetPosition;
		_sprite.GlobalPosition = avatar.SpritePosition;
		AvatarSkin = avatar.AvatarSkin;
		ChatterName = avatar.UserModel.display_name;
		Name = avatar.UserModel.id;
		ChatterColor = Globals.SavedChatColors[avatar.UserModel];
		_destMarker.Visible = false;
		_delay = avatar.Delay;
		UpdateState();
		_isMoving = IsMovingState(_state);
	}

	public bool IsMovingState(CurrentState state) => state is CurrentState.Walking or CurrentState.Running;
	public bool IsStillState(CurrentState state) => !IsMovingState(state);
	public bool IsStandingAnimation(CurrentState state) => state is CurrentState.SittingUp or CurrentState.LayingUp;
	private bool IsSolidPoint(Vector2I tilePos) => _grid.IsPointSolid(tilePos);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		_destMarker.Visible = false;
		var labelSettings = _chatterName.LabelSettings.Duplicate(true) as LabelSettings;
		_chatterName.LabelSettings = labelSettings;
		ChatterColor = _chatterColor;
		ChatterName = _chatterDisplayName;
		AvatarSkin = _avatarSkin;
		_grid = new AStarGrid2D();
		_grid.Offset = Map.GlobalPosition;
		_grid.Region = Map.GetUsedRect();
		_grid.CellSize = new Vector2(16, 16);
		_grid.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
		_grid.Update();

		var regionSize = _grid.Region.Size;
		var regionPosition = _grid.Region.Position;

		for (var x = 0; x < regionSize.X; x++)
		{
			for (var y = 0; y < regionSize.Y; y++)
			{
				var tilePos = new Vector2I(x + regionPosition.X, y + regionPosition.Y);
				var floorLayerData = Map.GetCellTileData(0, tilePos);
				var wallLayerData = Map.GetCellTileData(1, tilePos);
				var objectLayerData = Map.GetCellTileData(2, tilePos);
				var walkable = floorLayerData != null && floorLayerData.GetCustomData("walkable").AsBool();
				var isObject = objectLayerData != null && objectLayerData.GetCustomData("object").AsBool();
				var isWall = wallLayerData != null;
				if (!walkable) _grid.SetPointSolid(tilePos);
				if (isObject) _grid.SetPointSolid(tilePos);
				if (isWall) _grid.SetPointSolid(tilePos);
			}
		}
		
		_animTree.AnimationFinished += AnimTreeOnAnimationFinished;

		_state = CurrentState.Walking;
		_isMoving = true;
		_rng = new RandomNumberGenerator();
		_rng.Randomize();
		UpdateState();

		Globals.UpdateSkin += (id, skin) =>
		{
			if (id != Name && id != UserModel.id) return;
			if (skin != "") AvatarSkin = GD.Load<Texture2D>(skin);
			else EmitSignal(SignalName.RandomizeSkin);
		};
	}

	public override void _ExitTree()
	{
		GD.Print("Chatter leaving Tree.");
		var res = Globals.StreamAvatars.FirstOrDefault(x => x.UserModel.id == Name);
		if (res != null) Globals.StreamAvatars.Remove(res);
		Globals.StreamAvatars.Add(SnapCurrentState());
	}

	private void AnimTreeOnAnimationFinished(StringName animname)
	{
		if (!(_state is CurrentState.LayingUp or CurrentState.SittingUp)) return;
		_state = CurrentState.Deciding;
		_delay = _rng.RandfRange(3.0f, 15.0f);
		UpdateState();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isMoving) return;
		if (_state is CurrentState.Deciding)
		{
			_delay -= delta;
			if (_delay > 0.0) return;
			DecideNextState();
		}

		if (IsMovingState(_state)) return;
		if (IsStandingAnimation(_state)) return;
		_delay -= delta;
		if (_delay > 0.0) return;
		_state = _state switch
		{
			CurrentState.LayingDown => CurrentState.LayingUp,
			CurrentState.SittingDown => CurrentState.SittingUp,
			_ => CurrentState.Deciding
		};
		if (_state == CurrentState.Deciding) _delay = _rng.RandfRange(3.0f, 15.0f);
		UpdateState();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_isMoving) return;
		var path = _grid.GetIdPath(
			Map.LocalToMap(_sprite.GlobalPosition),
			Map.LocalToMap(GlobalPosition));
		
		if (path.Count > 0)
			path.RemoveAt(0);

		if (path.Count > 0)
		{
			var nextPos = Map.MapToLocal(path[0]);
			var diff = nextPos - _sprite.GlobalPosition;
			UpdateAnimationTree(diff);
			_sprite.GlobalPosition = _sprite.GlobalPosition.MoveToward(nextPos, _state == CurrentState.Running ? 1.0f : 0.5f);
			_isMoving = true;
		}
		else
		{
			if (_sprite.GlobalPosition != GlobalPosition)
			{
				_sprite.GlobalPosition = _sprite.GlobalPosition.MoveToward(GlobalPosition, 0.5f);
			}
			else
			{
				_destMarker.Visible = false;
				_isMoving = false;
				_state = CurrentState.Deciding;
				_delay = _rng.RandfRange(3.0f, 15.0f);
				UpdateState();
			}
		}
	}

	private void UpdateAnimationTree(Vector2 speed)
	{
		var treeSpeed = speed; //new Vector2();
		// Full Step (-1.0, 0.0, 1.0)
		// treeSpeed.X = MathF.Round(Math.Clamp(speed.X, -1.0f, 1.0f));
		// treeSpeed.Y = MathF.Round(Math.Clamp(speed.Y, -1.0f, 1.0f));

		_animTree.Set("parameters/anim_idle/blend_position", treeSpeed);
		_animTree.Set("parameters/anim_laying_down/blend_position", treeSpeed);
		_animTree.Set("parameters/anim_laying_up/blend_position", treeSpeed);
		_animTree.Set("parameters/anim_looking/blend_position", treeSpeed);
		_animTree.Set("parameters/anim_running/blend_position", treeSpeed);
		_animTree.Set("parameters/anim_sitting_down/blend_position", treeSpeed);
		_animTree.Set("parameters/anim_sitting_up/blend_position", treeSpeed);
		_animTree.Set("parameters/anim_walking/blend_position", treeSpeed);
	}

	private void DecideNextState()
	{
		var state = AvailableStates[_rng.RandiRange(0, AvailableStates.Count - 1)];

		if (state == _state) return;

		if (IsStillState(state))
			SetupStillState(state);
		else
			PickRandomSpot();
		_state = state;
		UpdateState();
	}

	private void UpdateState()
	{
		if (Globals.IsDebugging)
		{
			_currentState.Text = $"<{_state}>";
			_currentState.Visible = true;
		}
		else
		{
			_currentState.Visible = false;
		}

		_animTree.Set("parameters/state_laying/transition_request", _state == CurrentState.LayingDown ? "down" : "up");
		_animTree.Set("parameters/state_sitting/transition_request", _state == CurrentState.SittingDown ? "down" : "up");
		_animTree.Set("parameters/state_still/transition_request", _state switch
		{
			CurrentState.LayingDown or CurrentState.LayingUp=> "laying",
			CurrentState.SittingDown or CurrentState.SittingUp => "sitting",
			_ => "looking"
		});
		_animTree.Set("parameters/state_move/transition_request", _state == CurrentState.Walking ? "walking" : "running");
		_animTree.Set("parameters/state_main/transition_request", _state switch
		{
			CurrentState.LayingDown or CurrentState.LayingUp or CurrentState.SittingDown or CurrentState.SittingUp or CurrentState.LookingAround => "still_movement",
			CurrentState.Walking or CurrentState.Running => "movement",
			_ => "idle"
		});
	}

	private void PickRandomSpot()
	{
		_isMoving = true;
		var regionSize = _grid.Region.Size;
		var regionPosition = _grid.Region.Position;
		var pickedPosition = new Vector2I(_rng.RandiRange(0, regionSize.X -1), _rng.RandiRange(0, regionSize.Y - 1));
		while (IsSolidPoint(pickedPosition))
			pickedPosition = new Vector2I(_rng.RandiRange(0, regionSize.X -1), _rng.RandiRange(0, regionSize.Y - 1));
		_randomSpot = Map.MapToLocal(pickedPosition);
		var curPos = GlobalPosition;
		GlobalPosition = _randomSpot;
		_sprite.GlobalPosition = curPos;
		if (Globals.IsDebugging) _destMarker.Visible = true;
	}

	private void SetupStillState(CurrentState state)
	{
		_delay = _rng.RandfRange(3.0f, 15.0f);
		_isMoving = false;
	}

	public void SetSpawnPoint(Vector2 position)
	{
		var tmp = Map.LocalToMap(position);
		GlobalPosition = Map.MapToLocal(tmp);
	}

	public void SetSpecificPoint(Vector2 position)
	{
		var tmp = Map.LocalToMap(position);
		_randomSpot = Map.MapToLocal(tmp);
		var curPos = GlobalPosition;
		GlobalPosition = _randomSpot;
		_sprite.GlobalPosition = curPos;
		_isMoving = true;
		if (Globals.IsDebugging) _destMarker.Visible = true;
	} 
}

public enum CurrentState
{
	Deciding,
	Walking,
	Running,
	LayingDown,
	LayingUp,
	LookingAround,
	SittingDown,
	SittingUp
}