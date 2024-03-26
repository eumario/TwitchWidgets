namespace TwitchWidgets.Data.Models;

public class Secret
{
    public int Id { get; set; }
    public string ClientId { get; set; } = String.Empty;
    public string ClientSecret { get; set; } = String.Empty;
    
    #region Streamer Info
    public string? StreamerAuthToken { get; set; }
    public string? StreamerRefreshToken { get; set; }
    public string? StreamerUserId { get; set; }
    public string? StreamerUserName { get; set; }
    public string? StreamerDisplayName { get; set; }
    public string? StreamerProfilePic { get; set; }
    #endregion

    #region Bot Info
    public string? BotAuthToken { get; set; }
    public string? BotRefreshToken { get; set; }
    public string? BotUserId { get; set; }
    public string? BotUserName { get; set; }
    public string? BotDisplayName { get; set; }
    public string? BotProfilePic { get; set; }
    #endregion
}