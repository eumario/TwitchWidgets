namespace TwitchWidgets.Data.Models;

public class HeckleMessage
{
    public int Id { get; set; }
    public string Heckle { get; set; } = string.Empty;
    public int SuggestedId { get; set; } = -1;
    public bool Approved { get; set; } = false;

    public DateTime SuggestedAt { get; set; }
    public DateTime ApprovedAt { get; set; }
}