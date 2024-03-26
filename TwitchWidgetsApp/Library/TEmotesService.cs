using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using StreamingClient.Base.Util;
using TwitchWidgetsApp.Library.TEmotes;

namespace TwitchWidgetsApp.Library;

public class TEmotesService
{
    private List<string> _providerStrings = ["twitch", "7tv", "bttv", "ffz"];
    private string globalEmotes = "global/emotes/{0}"; // 0: Services (fmt: service.service)
    private string channelEmotes = "channel/{0}/emotes/{1}"; // 0: ChannelID, 1: Services
    private HttpClient _client;

    public TEmotesService()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://emotes.adamcy.pl/v1/")
        };
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<Emote>?> GetGlobalEmotes(List<Provider> providers)
    {
        var services = string.Join(".", providers.ConvertAll(x => (int)x).ConvertAll(x => _providerStrings[x]));
        var response = await _client.GetAsync(string.Format(globalEmotes,services));
        List<Emote>? emotes = null;
        if (response.IsSuccessStatusCode)
        {
            emotes = await response.ProcessResponse<List<Emote>>();
        }

        return emotes;
    }
    
    public async Task<List<Emote>?> GetChannelEmotes(string channelId, List<Provider> providers)
    {
        var services = string.Join(".", providers.ConvertAll(x => (int)x).ConvertAll(x => _providerStrings[x]));
        var response = await _client.GetAsync(string.Format(channelEmotes, channelId, services));
        List<Emote>? emotes = null;
        if (response.IsSuccessStatusCode)
        {
            emotes = await response.ProcessResponse<List<Emote>>();
        }

        return emotes;
    }
    
}