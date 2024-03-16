using Godot;
using Twitch.Base.Models.NewAPI.Users;

namespace TwitchWidgetsApp.Scenes.CatCafe.Scenes;

public record StreamAvatar
{
    public UserModel UserModel { get; set; }
    public CurrentState State { get; set; }
    public Vector2 TargetPosition { get; set; }
    public Vector2 SpritePosition { get; set; }
    public Texture2D AvatarSkin { get; set; }
    public double Delay { get; set; }
}