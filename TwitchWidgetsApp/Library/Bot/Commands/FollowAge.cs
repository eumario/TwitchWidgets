using System;
using System.Linq;
using Godot;
using NodaTime;
using Twitch.Base.Models.NewAPI.Channels;
using Twitch.Base.Models.NewAPI.Users;

namespace TwitchWidgetsApp.Library.Bot.Commands;

public class FollowAge : ICommand
{
    public TwitchWidgetsApp.Library.Globals Globals { get; set; }
    public bool Enabled => true;
    public string CommandText => "!followage";
    public string CommandAlias => "";
    public string CommandHelp => "Get's the amount of time you have followed this channel.";
    public string CommandDescription => "Get's the amount of time you have followed this channel.";

    public void Init()
    {
        // Noop
    }
    public async void RunCommand(UserModel model, string args, string messageId = "", bool isWhisper = false)
    {
        var streamer = Globals.Streamer;
        var conn = Globals.TwitchConnection;
        var chat = Globals.Chat;
        var api = $"channels/followers?broadcaster_id={streamer.id}&user_id={model.id}";
        var res = await conn.NewAPI.Channels.GetPagedDataResultAsync<ChannelFollowerModel>(api, 1);
        var user = res.First();
        if (user == null)
        {
            GD.Print("No user returned.");
            await chat.SendMessage(streamer, $"@{model.display_name}, you are currently not following.");
        }
        else
        {
            var date = LocalDateTime.FromDateTime(DateTime.Parse(user.followed_at));
            var now = LocalDateTime.FromDateTime(DateTime.UtcNow);
            var diff = Period.Between(date, now);
            GD.Print($"User has been following for {user.followed_at}");
            var humanTimespan = "";
            if (diff.Years > 0) humanTimespan += $"{diff.Years} years, ";
            if (diff.Months > 0) humanTimespan += $"{diff.Months} months, ";
            if (diff.Days > 0) humanTimespan += $"{diff.Days} days, ";
            if (diff.Hours > 0) humanTimespan += $"{diff.Hours} hours, ";
            if (diff.Minutes > 0) humanTimespan += $"{diff.Minutes} minutes.";
            
            await chat.SendReplyMessage(streamer,
                $"You have been following for {humanTimespan}", messageId);
        }
    }
}