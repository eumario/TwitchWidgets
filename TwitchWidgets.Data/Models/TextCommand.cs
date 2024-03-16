namespace TwitchWidgets.Data.Models;

public class TextCommand
{
    public int Id { get; set; }
    public string CommandName { get; set; }
    public string CommandAlias { get; set; }
    public string CommandHelp { get; set; }
    public string CommandDescription { get; set; }
}