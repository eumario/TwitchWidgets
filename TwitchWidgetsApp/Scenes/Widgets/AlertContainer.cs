using Godot;
using Godot.Sharp.Extras;
using Globals = TwitchWidgetsApp.Library.Globals;

namespace TwitchWidgetsApp.Scenes.Widgets;

[Tool]
[GlobalClass]
public partial class AlertContainer : PanelContainer
{
    [Singleton] public Globals? Globals;

    public override void _Ready()
    {
        this.OnReady();
    }
}