using Twitch.Base.Models.NewAPI.Users;

namespace TwitchWidgetsApp.Library.Bot.Commands;

public class EchoCommand : ICommand
{
    public TwitchWidgetsApp.Library.Globals Globals { get; set; }
    public bool Enabled => true;
    public string CommandText { get; }
    public string CommandAlias { get; }
    public string CommandHelp { get; }
    public string CommandDescription { get; }

    public EchoCommand(string commandText, string commandAlias, string commandHelp, string commandDescription)
    {
        CommandText = commandText;
        CommandAlias = commandAlias;
        CommandHelp = commandHelp;
        CommandDescription = commandDescription;
    }
    public void RunCommand(UserModel? model, string args, string messageId, bool isWhisper = false)
    {
        if (!isWhisper)
            Globals.TwitchManager.Chat.SendMessage(Globals.TwitchManager.Streamer, CommandDescription);
        else
            Globals.TwitchManager.Chat.SendWhisperMessage(Globals.TwitchManager.Streamer, model, CommandDescription);
    }
}