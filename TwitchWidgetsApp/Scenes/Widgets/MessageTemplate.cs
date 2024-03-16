using Godot;
using System;
using System.Collections.Generic;
using Godot.Sharp.Extras;
using Twitch.Base.Models.Clients.Chat;
using TwitchWidgetsApp.Library;

namespace TwitchWidgetsApp.Scenes.Widgets;

public partial class MessageTemplate : PanelContainer
{
	[Singleton] public Globals Globals;
	[NodePath] private TextureRect _userIcon;
	[NodePath] private Label _displayName;
	[NodePath] private Label _timeStamp;
	[NodePath] private RichTextLabel _message;
	[NodePath] private HBoxContainer _badges;

	private Texture2D _icon;
	private string _name;
	private DateTime _time;
	private string _msg;
	private Color _userColor;
	private List<ImageTexture> _badgeTextures = new();

	public Texture2D UserIcon
	{
		get => _icon;
		set
		{
			_icon = value;
			if (_userIcon != null)
				_userIcon.Texture = _icon;
		}
	}

	public string DisplayName
	{
		get => _name;
		set
		{
			_name = value;
			if (_displayName != null)
				_displayName.Text = value;
		}
	}

	public Color UserColor
	{
		get => _userColor;
		set
		{
			_userColor = value;
			if (_displayName != null)
				_displayName.LabelSettings.FontColor = value;
		}
	}

	public DateTime Timestamp
	{
		get => _time;
		set
		{
			_time = value;
			if (_timeStamp != null)
				_timeStamp.Text = $"{_time:MM/dd, hh:mm tt}";
		}
	}

	public string Message
	{
		get => _msg;
		set
		{
			_msg = value;
			if (_message != null)
				ParseBBCode(value);
				//_message.Text = value; // TODO: Need to Parse for Emoticons : EG: ParseBBCode(value); (Use PushXXXX/PopXXXX)
		}
	}

	public ChatMessagePacketModel MessagePacketModel;

	public void AddBadge(ImageTexture texture) => _badgeTextures.Add(texture);

	public void AddBadges(IEnumerable<ImageTexture> badges)
	{
		foreach (var badge in badges) AddBadge(badge);
	}
	
	public override void _Ready()
	{
		this.OnReady();
		UserIcon = _icon;
		UserColor = _userColor;
		DisplayName = _name;
		Timestamp = _time;
		Message = _msg;
		_displayName.LabelSettings = (LabelSettings)_displayName.LabelSettings.Duplicate(true);
		foreach(var node in _badges.GetChildren()) node.QueueFree();
		foreach (var badge in _badgeTextures)
		{
			var badgeRect = new TextureRect();
			badgeRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
			badgeRect.CustomMinimumSize = new Vector2(20, 20);
			badgeRect.Texture = badge;
			_badges.AddChild(badgeRect);
		}
	}

	private async void ParseBBCode(string msg)
	{
		//TODO: Need to implement FrankerFaceZ, BetterTTV and 7TV, along with Getting Missing Emoticons.
		_message.Text = "";
		var keyedIndex = new Dictionary<string, string>();
		var cached = new Dictionary<string, ImageTexture>();
		foreach (var (emote, positions) in MessagePacketModel.EmotesDictionary)
		{
			var emText = Globals.ImageManager.GetTwitchEmoteTexture(emote) ?? await Globals.ImageManager.FetchTwitchEmote(emote);

			var (start, end) = positions[0];
			keyedIndex[msg.Substr(start, end - start + 1)] = emote;
			cached[emote] = emText;
		}

		// msg = keyedIndex.Keys.Aggregate(msg, (current, mote) => current.Replace(mote, $"[emote]{keyedIndex[mote]}[/emote]"));
		foreach (var mote in keyedIndex.Keys)
		{
			msg = msg.Replace(mote, $"[emote]{keyedIndex[mote]}[/emote]");
		}

		foreach (var part in msg.Split(" "))
		{
			if (part.StartsWith("[emote]")) continue;
			if (!Globals.ImageManager.Has3rdEmote(part)) continue;
			if (cached.ContainsKey(part)) continue;
			cached[part] = Globals.ImageManager.Get3rdEmote(part);
			msg = msg.Replace(part, $"[emote]{part}[/emote]");
		}

		foreach (var part in msg.Split("[emote]"))
		{
			if (part.Contains("[/emote]"))
			{
				var emote = part.Replace("[/emote]", "").StripEdges();
				var image = cached[emote];
				if (image == null)
					_message.AddText(emote);
				else
					_message.AddImage(image);
			}
			else
			{
				_message.AddText(part);
			}
		}
		
	}
}
