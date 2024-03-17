using Twitch.Base.Models.NewAPI.Users;

namespace TwitchWidgetsApp.Library.Bot;

public interface ICommand
{
    public TwitchWidgetsApp.Library.Globals Globals { get; set; }
    public bool Enabled { get; }
    public string CommandText { get; }
    public string CommandAlias { get; }
    public string CommandHelp { get; }
    public string CommandDescription { get; }
    
    public void RunCommand(UserModel model, string args, string messageId, bool isWhisper = false);
    public void Init();
}