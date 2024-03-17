using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Sharp.Extras;
using Twitch.Base.Models.NewAPI.Chat;
using TwitchWidgetsApp.Library.TEmotes;

namespace TwitchWidgetsApp.Library.Managers;

public partial class ImageManager : Node
{
    [Singleton] public Globals Globals;
    private Dictionary<ChatBadgeSetModel, Dictionary<ChatBadgeModel, ImageTexture>> _chatBadges = new();
    private Dictionary<ChatEmoteModel, ImageTexture> _twitchEmotes = new();
    private Dictionary<string, ImageTexture> _3rdPartyEmotes = new();
    private TEmotesService _3rdPartyProvider = new();

    public bool LoadedBadges = false;
    public bool LoadedTwitch = false;
    public bool Loaded3rdParty = false;

    public override void _Ready()
    {
        this.OnReady();
    }

    public ImageTexture? GetTwitchEmoteTexture(string part)
    {
        var kvp = _twitchEmotes.Keys.FirstOrDefault(x => x.id == part);
        return kvp == null ? null : _twitchEmotes[kvp];
    }

    public async Task<ImageTexture?> FetchTwitchEmote(string emote)
    {
        var test = GetTwitchEmoteTexture(emote);
        if (test != null) return test;
        
        ChatEmoteModel emoteModel = new()
        {
            id = emote
        };
        var img = await Util.FetchImage(emoteModel.BuildImageURL("static", "dark", "1.0"), 
            "twitchEmotes", $"{emoteModel.id}.png");
        _twitchEmotes[emoteModel] = img;
        return img;
    }
    
    public async Task FetchTwitchEmotes()
    {
        var globalEmotes = await Globals.TwitchApi.Chat.GetGlobalEmotes();
        var channelEmotes =
            await Globals.TwitchApi.Chat.GetChannelEmotes(Globals.Streamer);

        var printOnce = false;
        foreach (var emote in globalEmotes)
        {
            if (!printOnce)
            {
                printOnce = true;
            }
            var img = await Util.FetchImage(emote.BuildImageURL("static", "dark", "1.0"),
                "twitchEmotes", $"{emote.id}.png");
            _twitchEmotes[emote] = img;
        }

        foreach (var emote in channelEmotes)
        {
            var img = await Util.FetchImage(emote.BuildImageURL("static", "dark", "1.0"),
                "twitchEmotes", $"{emote.id}.png");
            _twitchEmotes[emote] = img;
        }

        LoadedTwitch = true;
    }

    public ImageTexture? GetBadgeTexture(string set_id, string id)
    {
        var badgeSet = _chatBadges.Keys.FirstOrDefault(x => x.set_id == set_id);
        if (badgeSet == null)
            return null;

        var badge = _chatBadges[badgeSet].Keys.FirstOrDefault(x => x.id == id);
        return badge == null ? null : _chatBadges[badgeSet][badge];
    }

    public async Task FetchTwitchBadges()
    {
        var globalBadges = await Globals.TwitchApi.Chat.GetGlobalChatBadges();
        var channelBadges =
            await Globals.TwitchApi.Chat.GetChannelChatBadges(Globals.Streamer);

        foreach (var badgeSet in globalBadges)
        {
            _chatBadges[badgeSet] = new Dictionary<ChatBadgeModel, ImageTexture>();
            foreach (var (badge, index) in badgeSet.versions.WithIndex())
            {
                var img = await Util.FetchImage(badge.image_url_4x, "twitchBadges", $"{badgeSet.set_id}.{index}.png");
                _chatBadges[badgeSet][badge] = img;
            }
        }

        foreach (var badgeSet in channelBadges)
        {
            _chatBadges[badgeSet] = new Dictionary<ChatBadgeModel, ImageTexture>();
            foreach (var (badge, index) in badgeSet.versions.WithIndex())
            {
                var img = await Util.FetchImage(badge.image_url_4x, "twitchBadges", $"{badgeSet.set_id}.{index}.png");
                _chatBadges[badgeSet][badge] = img;
            }
        }

        LoadedBadges = true;
    }
    
    public bool Has3rdEmote(string part) => _3rdPartyEmotes.ContainsKey(part);

    public ImageTexture? Get3rdEmote(string part) => _3rdPartyEmotes[part];

    public async Task Fetch3rdPartyEmotes()
    {
        var globalEmotes = 
            await _3rdPartyProvider.GetGlobalEmotes([Provider.BetterTV, Provider.FrankerFaceZ, Provider.SevenTV]);
        var channelEmotes = await _3rdPartyProvider.GetChannelEmotes(Globals.Streamer.id,
            [Provider.BetterTV, Provider.FrankerFaceZ, Provider.SevenTV]);

        foreach (var emote in globalEmotes)
        {
            var url = emote.Get1x();
            var fileName = string.Join("_", url.Split("/")[^2..]);
            var img = await Util.FetchImage(url, "3rdPartyEmotes", fileName);
            _3rdPartyEmotes[emote.Code] = img;
        }

        if (channelEmotes == null) return;

        foreach (var emote in channelEmotes)
        {
            var url = emote.Get1x();
            var fileName = string.Join("_", url.Split("/")[^2..]);
            var img = await Util.FetchImage(url, "3rdPartyEmotes", fileName);
            _3rdPartyEmotes[emote.Code] = img;
        }

        Loaded3rdParty = true;
    }
}