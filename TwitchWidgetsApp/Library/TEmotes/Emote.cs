using System.Collections.Generic;
using System.Linq;

namespace TwitchWidgetsApp.Library.TEmotes;

public class Emote
{
    public Provider Provider { get; set; }
    public string? Code { get; set; }
    public List<Dictionary<string, string>> Urls { get; set; } = [];

    private string? GetUrl(string size, bool dark = false)
    {
        var url = Urls.FirstOrDefault(x => x["size"] == size);
        if (url == null) return null;
        if (Provider == Provider.Twitch && dark)
            return url["url"].Replace("/light/", "/dark/");
        else
            return url["url"];

    }

    public string? Get1x(bool dark = false) => GetUrl("1x", dark);
    public string? Get2x(bool dark = false) => GetUrl("2x", dark);
    public string? Get3x(bool dark = false) => Provider == Provider.Twitch ? GetUrl("2x", dark) : GetUrl("3x", dark);
    public string? Get4x(bool dark = false) => GetUrl("4x", dark);
}

public enum Provider
{
    Twitch = 0,
    SevenTV,
    BetterTV,
    FrankerFaceZ
}