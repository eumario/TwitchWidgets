using Godot;
using Godot.Sharp.Extras;

namespace TwitchWidgetsApp.Library.Controls;
public partial class ObsSettings : GridContainer
{
	[Singleton] public Globals? Globals;
	[NodePath] private LineEdit? _host;
	[NodePath] private LineEdit? _port;
	[NodePath] private LineEdit? _password;

	private string? _sHost;
	private string? _sPort;
	private string? _sPass;

	public string Host
	{
		get => _host == null ? _sHost! : _host.Text;
		set
		{
			_sHost = value;
			if (_host != null) _host.Text = value;
		}
	}

	public string Port
	{
		get => _port == null ? _sPort! : _port.Text;
		set
		{
			_sPort = value;
			if (_port != null) _port.Text = value;
		}
	}

	public string Password
	{
		get => _password == null ? _sPass! : _password.Text;
		set
		{
			_sPass = value;
			if (_password != null) _password.Text = value;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		Host = _sHost!;
		Password = _sPass!;
		Port = _sPort!;
	}
}
