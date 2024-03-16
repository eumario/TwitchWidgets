using Godot;
using System.Collections.Generic;
using Godot.Sharp.Extras;
using TwitchWidgetsApp.Library;
using TwitchWidgetsApp.Library.Controls.Effects;

namespace TwitchWidgetsApp.Scenes.Widgets;

public partial class MusicPlayer : Control
{
	[Singleton] public Globals Globals;

	[NodePath] private TextureRect _albumIcon;
	[NodePath] private Label _songArtist;
	[NodePath] private Label _songTitle;
	[NodePath] private Label _plCurrent;
	[NodePath] private Label _plTotal;
	[NodePath] private Label _currentTime;
	[NodePath] private Label _songLength;
	[NodePath] private ProgressBar _songProgress;
	[NodePath] private SpectrumAnalyzer _spectrumAnalyzer;

	private ulong _albumIconId;

	private Dictionary<string, ImageTexture> _cacheAlbumArt = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		_albumIconId = _albumIcon.GetInstanceId();
		Globals.MusicController.SongChanged += MusicControllerOnSongChanged;
		Globals.MusicController.PauseChanged += MusicControllerOnSongChanged;
		MusicControllerOnSongChanged();
	}

	public override void _ExitTree()
	{
		Globals.MusicController.SongChanged -= MusicControllerOnSongChanged;
		Globals.MusicController.PauseChanged -= MusicControllerOnSongChanged;
	}


	private void MusicControllerOnSongChanged()
	{
		if (!IsInstanceIdValid(_albumIconId))
		{
			Globals.MusicController.SongChanged -= MusicControllerOnSongChanged;
			Globals.MusicController.PauseChanged -= MusicControllerOnSongChanged;
			return;
		}
		var info = Globals.MusicController.CurrentSongTag;
		if (info == null) { Globals.RunOnMain(MusicControllerOnSongChanged); return; }
		if (_cacheAlbumArt.ContainsKey(info.AlbumArt))
			_albumIcon.Texture = _cacheAlbumArt[info.AlbumArt];
		else
		{
			var img = Image.LoadFromFile(info.AlbumArt);
			var texture = ImageTexture.CreateFromImage(img);
			_cacheAlbumArt[info.AlbumArt] = texture;
			_albumIcon.Texture = texture;
		}

		_songArtist.Text = info.Artist;
		_songTitle.Text = info.Title;
		_plCurrent.Text = Globals.MusicController.CurrentSongIndex.ToString();
		_plTotal.Text = Globals.MusicController.PlaylistCount.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Globals.MusicController.IsPlaying) return;
		_songLength.Text = $"{Globals.MusicController.CurrentTrackLength:mm\\:ss}";
		_currentTime.Text = $"{Globals.MusicController.CurrentTrackTime:mm\\:ss}";
		_songProgress.MaxValue = Globals.MusicController.CurrentTrackTotal;
		_songProgress.Value = Globals.MusicController.CurrentTrackPosition;
	}
}
