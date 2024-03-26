namespace TwitchWidgets.Data.Models;

public class TextCommand
{
    public int Id { get; set; }
    public string CommandName { get; set; } = String.Empty;
    public string CommandAlias { get; set; } = String.Empty;
    public string CommandHelp { get; set; } = String.Empty;
    public string CommandDescription { get; set; } = String.Empty;
}