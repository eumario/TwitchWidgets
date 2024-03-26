using Godot;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.NewAPI.Users;
using TwitchWidgets.Data.Models;

namespace TwitchWidgetsApp.Library.Bot.Commands;

public class HeckleCommand : ICommand
{
    public Globals? Globals { get; set; }
    public bool Enabled => true;
    public string CommandText => "!heckle";
    public string CommandAlias => "!heck";
    public string CommandHelp => "Toss a Heckle to the streamer.";
    public string CommandDescription => "Ensures that the streamer is staying humble.";

    private List<HeckleMessage> _heckles = [];
    private RandomNumberGenerator _rng;

    public HeckleCommand()
    {
        _rng = new RandomNumberGenerator();
        _rng.Randomize();
    }

    private void LoadHeckles()
    {
        if (Globals!.Database?.HeckleMessages == null) { Globals.RunOnMain(LoadHeckles); return; }
        
        foreach(var heckle in Globals.Database.HeckleMessages.ToList())
            _heckles.Add(heckle);
    }

    public void Init()
    {
        LoadHeckles();
    }

    public void RunCommand(UserModel model, string args, string messageId, bool isWhisper = false)
    {
        var heckle = _heckles[_rng.RandiRange(0, _heckles.Count - 1)];
        Globals!.Chat.SendMessage(Globals.Streamer, heckle.Heckle);
    }
}