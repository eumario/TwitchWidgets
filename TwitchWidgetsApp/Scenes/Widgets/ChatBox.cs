using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Sharp.Extras;
using StreamingClient.Base.Util;
using Twitch.Base.Models.Clients.Chat;
using TwitchWidgetsApp.Library;

namespace TwitchWidgetsApp.Scenes.Widgets;

[Tool]
[GlobalClass]
public partial class ChatBox : PanelContainer
{
    [Singleton] public Globals Globals;
    [Export] public PackedScene MessageTemplate;
    public VBoxContainer ChatHistory;
    public ScrollContainer Scroll;
    public MarginContainer Margin;

    private double _maxScroll = 0;
    private bool _isLoading = false;
    private ulong _chId;
    

    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;
        this.OnReady();
        if (Globals.TwitchManager.IsReady)
        {
            Task.Run(async () =>
            {
                Thread.Sleep(500);
                SetupEvents();
                await FetchImages();
            });
        }
        Globals.TwitchManager.ManagerReady += async () =>
        {
            SetupEvents();
            await FetchImages();
        };

        Scroll = new ScrollContainer();
        Scroll.GetVScrollBar().Changed += () =>
        {
            var res = Scroll.GetVScrollBar().MaxValue;
            if (!(res > _maxScroll)) return;
            _maxScroll = res;
            Scroll.GetVScrollBar().Value = _maxScroll;
        };
        ChatHistory = new VBoxContainer();
        _chId = ChatHistory.GetInstanceId();
        ChatHistory.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        ChatHistory.SizeFlagsVertical = SizeFlags.ExpandFill;
        Margin = new MarginContainer();
        Scroll.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
        Scroll.SizeFlagsHorizontal = SizeFlags.ExpandFill;

        Margin.BeginBulkThemeOverride();
        Margin.AddThemeConstantOverride("margin_left", 10);
        Margin.AddThemeConstantOverride("margin_right", 10);
        Margin.AddThemeConstantOverride("margin_top", 10);
        Margin.AddThemeConstantOverride("margin_bottom", 10);
        Margin.EndBulkThemeOverride();

        Margin.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        Margin.SizeFlagsVertical = SizeFlags.ExpandFill;
        
        AddChild(Margin);
        Margin.AddChild(Scroll);
        Scroll.AddChild(ChatHistory);

        Task.Run(async () =>
        {
            await this.IdleFrame();
            LoadHistory();
        });
    }

    public override void _ExitTree()
    {
        if (Engine.IsEditorHint()) return;
        Globals.Chat.OnMessageReceived -= ChatMessageReceived;
    }

    private async Task FetchImages()
    {
        if (!Globals.ImageManager.LoadedBadges) await Globals.ImageManager.FetchTwitchBadges();
        if (!Globals.ImageManager.LoadedTwitch) await Globals.ImageManager.FetchTwitchEmotes();
        if (!Globals.ImageManager.Loaded3rdParty) await Globals.ImageManager.Fetch3rdPartyEmotes();
    }

    private void SetupEvents()
    {
        Globals.Chat.OnMessageReceived += ChatMessageReceived;
    }

    private void ChatMessageReceived(object? sender, ChatMessagePacketModel model)
    {
        AddMessage(model);
    }

    private async void AddMessage(ChatMessagePacketModel message)
    {
        if (!IsInstanceIdValid(_chId)) return;
        var user = Globals.Chatters.FirstOrDefault(x => x.id == message.UserID);
        if (user == null)
        {
            Globals.RunOnMain(() => AddMessage(message));
            return;
        }

        if (!Globals.SavedChatColors.ContainsKey(user))
        {
            Globals.RunOnMain(() => AddMessage(message));
            return;
        }

        var img = await Util.FetchImage(user.profile_image_url);
        var color = Globals.SavedChatColors[user];
        var msg = message.Message;
        var name = message.UserDisplayName;
        var timestamp = long.Parse(message.Timestamp);

        var badges = new List<ImageTexture>();
        foreach (var (set_id, version) in message.BadgeDictionary)
        {
            var image = Globals.ImageManager.GetBadgeTexture(set_id, version.ToString());
            if (image == null) continue;
            badges.Add(image);
        }

        var tmpl = MessageTemplate.Instantiate<MessageTemplate>();
        tmpl.MessagePacketModel = message;
        tmpl.UserIcon = img;
        tmpl.UserColor = color;
        tmpl.DisplayName = name;
        tmpl.Message = msg;
        tmpl.Timestamp = DateTimeOffsetExtensions.FromUTCUnixTimeMilliseconds(timestamp).DateTime;
        tmpl.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        tmpl.SizeFlagsVertical = SizeFlags.ShrinkBegin;
        tmpl.AddBadges(badges);
        if (!IsInstanceIdValid(_chId))
        {
            tmpl.QueueFree();
            return;
        }
        ChatHistory.AddChild(tmpl);
    }

    private void LoadHistory()
    {
        foreach(var node in ChatHistory.GetChildren()) node.QueueFree();
        
        foreach(var message in Globals.ChatHistory)
            AddMessage(message);
    }
}