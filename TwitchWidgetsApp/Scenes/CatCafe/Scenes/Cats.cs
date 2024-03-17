using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Godot.Sharp.Extras;
using Twitch.Base.Models.Clients.Chat;
using TwitchWidgetsApp.Library;

namespace TwitchWidgetsApp.Scenes.CatCafe.Scenes;

public partial class Cats : Node2D
{
	[Singleton] public Globals Globals;
	[Export] private Node2D _spawnPosition;
	[Export] private Node2D _initialWalkingPosition;
	[Export] private TileMap _map;

	[Export(PropertyHint.Dir)] private string _avatarPath;

	private RandomNumberGenerator _rng;

	private List<Texture2D> _avatarSkins = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		Initialize();

		var dir = DirAccess.Open(_avatarPath);
		foreach (var file in dir.GetFiles())
		{
			if (!file.EndsWith(".png")) continue;
			_avatarSkins.Add(GD.Load<Texture2D>(Path.Join(_avatarPath, file)));
		}

		_rng = new RandomNumberGenerator();
		_rng.Randomize();
		
		Globals.RunOnMain(() => PopulateCafe());
	}

	private void PopulateCafe()
	{
	}

	public override void _ExitTree()
	{
		Globals.Chat.OnMessageReceived -= HandleChatMessage;
	}

	private void Initialize()
	{
		if (Globals.TwitchManager == null) { Globals.RunOnMain(Initialize); return; }
		if (Globals.Chat == null) { Globals.RunOnMain(Initialize); return; }
		Globals.Chat.OnMessageReceived += HandleChatMessage;
		Globals.RunOnMain(PopulateAvatars);
	}

	private void PopulateAvatars()
	{
		foreach (var avatar in Globals.StreamAvatars)
		{
			var chatter = Chatter.Create();
			chatter.Map = _map;
			AddChild(chatter);
			chatter.RestoreState(avatar);
		}
	}

	private void HandleChatMessage(object sender, ChatMessagePacketModel e)
	{
		if (IsQueuedForDeletion()) return;
		var node = GetNodeOrNull<Chatter>(e.UserID);
		if (node != null) return;
		var user = Globals.Chatters.FirstOrDefault(x => x.id == e.UserID);
		if (user == null)
		{
			Globals.RunOnMain(() => HandleChatMessage(sender, e));
			return;
		}
		
		if (!Globals.SavedChatColors.ContainsKey(user))
		{
			Globals.RunOnMain(() => HandleChatMessage(sender, e));
			return;
		}

		var kc = Globals.KnownChatters.FirstOrDefault(x => x.TwitchId == e.UserID);
		var avatar = "";
		if (kc != null) avatar = kc.StreamAvatar;
		
		// Need to Spawn our cat for this user.
		var chatter = Chatter.Create();
		chatter.Name = e.UserID;
		chatter.Map = _map;
		chatter.ChatterColor = Globals.SavedChatColors[user];
		chatter.ChatterName = user.display_name;
		chatter.AvatarSkin = avatar == ""
			? _avatarSkins[_rng.RandiRange(0, _avatarSkins.Count - 1)]
			: GD.Load<Texture2D>(avatar);
		chatter.UserModel = user;
		AddChild(chatter);
		chatter.SetSpawnPoint(_spawnPosition.GlobalPosition);
		chatter.SetSpecificPoint(_initialWalkingPosition.GlobalPosition);
		chatter.RandomizeSkin += () => chatter.AvatarSkin = _avatarSkins[_rng.RandiRange(0, _avatarSkins.Count - 1)];
	}
}
