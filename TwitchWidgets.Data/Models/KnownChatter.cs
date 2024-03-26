namespace TwitchWidgets.Data.Models;

public class KnownChatter
{
    public int Id { get; set; }
    public string TwitchId { get; set; } = String.Empty;
    public string DisplayName { get; set; } = String.Empty;
    public string AvatarUrl { get; set; } = String.Empty;
    public string StreamAvatar { get; set; } = String.Empty;
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
}