using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Sharp.Extras;
using Twitch.Base.Models.Clients.Chat;
using TwitchWidgetsApp.Library.Bot;
using TwitchWidgetsApp.Library.Bot.Commands;

namespace TwitchWidgetsApp.Library.Managers;

public partial class BotManager : Node
{
    [Singleton] public Globals Globals;

    private List<ICommand> _commands = new();

    public override void _Ready()
    {
        this.OnReady();
        SetupEvents();
        var type = typeof(ICommand);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p));
        foreach (var t in types)
        {
            if (t == typeof(EchoCommand)) continue;
            if (t == typeof(ICommand)) continue;
            var inst = (ICommand)Activator.CreateInstance(t);
            inst.Globals = Globals;
            inst.Init();
            _commands.Add(inst);
        }

        LoadEchoCommands();
        Globals.UpdateCommands += LoadEchoCommands;
    }

    private void LoadEchoCommands()
    {
        if (Globals.Database?.Commands == null) { Globals.RunOnMain(LoadEchoCommands); return; }
        
        _commands.RemoveAll(x => x is EchoCommand);
        
        foreach (var tcmd in Globals.Database.Commands)
        {
            var ecmd = new EchoCommand(tcmd.CommandName, tcmd.CommandAlias, tcmd.CommandHelp, tcmd.CommandDescription);
            ecmd.Globals = Globals;
            ecmd.Init();
            _commands.Add(ecmd);
        }
    }

    private void SetupEvents()
    {
        if (Globals.TwitchManager == null) { Globals.RunOnMain(SetupEvents); return; }
        if (Globals.Chat == null) { Globals.RunOnMain(SetupEvents); return; }
        
        Globals.Chat.OnMessageReceived += ProcessMessage;
        Globals.Chat.OnWhisperMessageReceived += ProcessWhisper;
    }

    private void ProcessWhisper(object sender, ChatWhisperMessagePacketModel e)
    {
        var msg = e.Message;
        var cmd = msg.Split(" ")[0];
        var args = msg.Replace(cmd, "").Trim();
        var icmd = _commands.FirstOrDefault(x => x.CommandText == cmd || x.CommandText.Contains(cmd));
        if (icmd == null) return;
        var chatter = Globals.Chatters.FirstOrDefault(x => x.id == e.UserID);
        if (chatter == null)
        {
            Globals.RunOnMain(() => ProcessWhisper(sender, e));
            return;
        }
        if (icmd.Enabled) icmd.RunCommand(chatter, args, e.ID, true);
    }

    private void ProcessMessage(object sender, ChatMessagePacketModel e)
    {
        var msg = e.Message;
        var cmd = msg.Split(" ")[0];
        var args = msg.Replace(cmd, "").Trim();
        var icmd = _commands.FirstOrDefault(x => x.CommandText == cmd || x.CommandAlias.Contains(cmd));
        if (icmd == null) return;
        var chatter = Globals.Chatters.FirstOrDefault(x => x.id == e.UserID);
        if (chatter == null)
        {
            Globals.RunOnMain(() => ProcessMessage(sender, e));
            return;
        }
        if (icmd.Enabled) icmd.RunCommand(chatter, args, e.ID);
    }
}