using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Twitch.Base.Models.NewAPI.Users;

namespace TwitchWidgetsApp.Library.Bot.Commands;

public class SetAvatar : ICommand
{
    public Globals Globals { get; set; }
    public bool Enabled => true;
    public string CommandText => "!avatar";
    public string CommandAlias => "!setavatar;!setav";
    public string CommandHelp => "Set's your Cat Avatar in the Cafe.";
    public string CommandDescription => "Set's a user's avatar for Stream Avatar.";

    private string _avatarPath = "res://Scenes/CatCafe/Assets/Cats/";
    private List<string> _avatarSkins = [];
    private List<string> _shortList = [];

    public SetAvatar()
    {
        var dh = DirAccess.Open(_avatarPath);
        if (dh == null) return;
        var files = dh.GetFiles().ToList().FindAll(x => x.EndsWith(".png"));
        _avatarSkins.AddRange(files);
        files = files.Select(x => x[..x.LastIndexOf("_", StringComparison.Ordinal)]).Distinct().ToList();
        _shortList.AddRange(files);
    }

    public void Init()
    {
        // Noop
    }
    public void RunCommand(UserModel model, string args, string messageId, bool isWhisper = false)
    {
        if (args is "" or "list")
        {
            var msg = "Skins List: " + _shortList.Aggregate((a, b) => a.Replace(".png","") + ", " + b.Replace(".png",""));
            GD.Print($"Sending> {msg} ({msg.Length} characters)");
            Globals.Chat.SendReplyMessage(Globals.Streamer, msg, messageId);
        }
        else if (args is "reset" or "random")
        {
            var kc = Globals.KnownChatters.FirstOrDefault(x => x.TwitchId == model.id);
            if (kc is null)
                Globals.Chat.SendReplyMessage(Globals.Streamer,
                    "Unable to update your skin.", messageId);
            else
            {
                kc.StreamAvatar = "";
                Globals.Database.SaveChanges();
                Globals.EmitSignal(Globals.SignalName.UpdateSkin, model.id, "");
                Globals.Chat.SendReplyMessage(Globals.Streamer, "Skin reset to random.", messageId);
            }
        }
        else
        {
            var skin = _shortList.FirstOrDefault(x => x.ToLower() == args.ToLower());
            if (skin is null)
            {
                Globals.Chat.SendReplyMessage(Globals.Streamer, "Skin not found. Use !avatar list to see a list of skins.", messageId);
            }
            else
            {
                var path = _avatarPath + skin + "_0.png";
                var kc = Globals.KnownChatters.FirstOrDefault(x => x.TwitchId == model.id);
                if (kc is null)
                    Globals.Chat.SendReplyMessage(Globals.Streamer,
                        "Unable to update your skin.", messageId);
                else
                {
                    kc.StreamAvatar = path;
                    Globals.Database.SaveChanges();
                    Globals.EmitSignal(Globals.SignalName.UpdateSkin, model.id, path);
                    Globals.Chat.SendReplyMessage(Globals.Streamer, $"Skin updated to {args}.", messageId);
                }
            }
        }
    }
}