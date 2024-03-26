using Godot;
using Godot.Sharp.Extras;
using TwitchWidgetsApp.Resources.AlertCollections.ForestAlerts;

namespace TwitchWidgetsApp.Library.Controls;

[GlobalClass]
public partial class ObsScene : Control
{
    [Singleton] public Globals? Globals;

    public override void _Ready()
    {
        this.OnReady();
        Globals!.AlertManager!.ShowAlert += AlertManagerOnShowAlert;
    }

    private void AlertManagerOnShowAlert(AlertScript alert)
    {
        AddChild(alert);
    }

    public override void _ExitTree()
    {
        Globals!.AlertManager!.ShowAlert -= AlertManagerOnShowAlert;
    }
}