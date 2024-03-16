namespace TwitchWidgets.Data.Models;

public class KnownChatter
{
    public int Id { get; set; }
    public string TwitchId { get; set; }
    public string DisplayName { get; set; }
    public string AvatarUrl { get; set; }
    public string StreamAvatar { get; set; }
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
}