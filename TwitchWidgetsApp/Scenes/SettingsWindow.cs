using Godot;
using Godot.Sharp.Extras;
using Globals = TwitchWidgetsApp.Library.Globals;
namespace TwitchWidgetsApp.Scenes;

public partial class SettingsWindow : Control
{
	[Singleton] public Globals Globals;

	[NodePath] private TabContainer _settingsContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		_settingsContainer.TabChanged += tab =>
		{
			Globals.SettingsManager.SetValue("last_settings_tab", (int)tab);
		};
		_settingsContainer.CurrentTab = Globals.SettingsManager.GetValue("last_settings_tab", 0);
	}
}
