using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Godot.Sharp.Extras;

namespace TwitchWidgetsApp.Library.Managers;

public partial class MusicController : Node
{
    [Singleton] public Globals? Globals;
    [Singleton] public Node? ElgatoStreamDeck;

    [Signal] public delegate void SongChangedEventHandler();
    [Signal] public delegate void PauseChangedEventHandler();
    
    private AudioStreamPlayer? _player;
    public List<string> _songList = [];
    public Queue<string> _playlist = [];
    public Dictionary<string, AudioStream> _cacheSongs = [];
    public Dictionary<string, TagInfo> _tags = [];
    public TagInfo? CurrentSongTag;
    public int CurrentSongIndex = 0;
    public int PlaylistCount => _songList.Count;

    private int _bus_index;
    private string? _currSong;
    private float _curVol = 1.0f;

    public TimeSpan CurrentTrackLength =>
        _currSong == null ? TimeSpan.Zero : TimeSpan.FromSeconds(_player!.Stream.GetLength());
    public TimeSpan CurrentTrackTime =>
        _currSong == null ? TimeSpan.Zero : TimeSpan.FromSeconds(_player!.GetPlaybackPosition());
    public float CurrentTrackPosition => _currSong == null ? 0.0f : _player!.GetPlaybackPosition();
    public float CurrentTrackTotal => _currSong == null ? 0.0f : (float)_player!.Stream.GetLength();

    public bool IsPlaying => _player!.Playing;

    public AudioEffectSpectrumAnalyzerInstance GetSpectrum() => (AudioEffectSpectrumAnalyzerInstance)AudioServer.GetBusEffectInstance(_bus_index, 0);

    public override void _Ready()
    {
        this.OnReady();
        ElgatoStreamDeck!.Connect("on_key_down", Callable.From<string>(HandleElgato));
        _player = new AudioStreamPlayer
        {
            Name = "MusicControllerPlayer",
            Bus = "music"
        };
        _bus_index = AudioServer.GetBusIndex("music");
        AddChild(_player);
        Globals!.SettingsLoaded += OnSettingsLoaded;
        Globals.SettingsUpdated += OnSettingsUpdated;
        _player.Finished += PlayerOnFinished;
    }

    private void PlayerOnFinished()
    {
        CurrentSongIndex++;
        if (CurrentSongIndex > _songList.Count) CurrentSongIndex = 1;
        var song = _playlist.Dequeue();
        _currSong = song;
        if (!_cacheSongs.ContainsKey(song))
            LoadSong(song);
        _player!.Stream = _cacheSongs[song];
        CurrentSongTag = _tags[song];
        if (Globals!.LoopMusic)
            _playlist.Enqueue(song);
        EmitSignal(SignalName.SongChanged);
        _player.Play();
    }

    private void LoadSong(string song)
    {
        if (song.EndsWith(".mp3"))
        {
            FetchTag(song);
            var mp3 = new AudioStreamMP3
            {
                Data = File.ReadAllBytes(song)
            };
            _cacheSongs[song] = mp3;
        }
        else if (song.EndsWith(".wav"))
        {
            var wav = new AudioStreamWav
            {
                Data = File.ReadAllBytes(song)
            };
            _cacheSongs[song] = wav;
        }
        else if (song.EndsWith(".ogg"))
        {
            FetchTag(song);
            var ogg = AudioStreamOggVorbis.LoadFromFile(song);
            _cacheSongs[song] = ogg;
        }
    }

    private void FetchTag(string song)
    {
        if (_tags.ContainsKey(song)) return;
        var info = TagLib.File.Create(song);
        if (info.Tag.Pictures.Length > 0)
        {
            var fname = $"{info.Tag.FirstAlbumArtist} - {info.Tag.Album}.png";
            var path = Path.GetFullPath(Path.Combine(OS.GetUserDataDir(), "cache", "albumArtwork"));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (!File.Exists(Path.Combine(path, fname)))
            {
                File.WriteAllBytes(Path.Combine(path, fname), info.Tag.Pictures[0].Data.Data);
            }

            var tag = new TagInfo(info.Tag.FirstAlbumArtist, info.Tag.Title, Path.Combine(path, fname));
            _tags[song] = tag;
        }
    }

    private void OnSettingsLoaded()
    {
        _songList.Clear();
        foreach (var file in Globals!.MusicFolders.SelectMany(path => Directory.EnumerateFiles(path)))
        {
            if (file.EndsWith(".mp3")) _songList.Add(file);
            if (file.EndsWith(".ogg")) _songList.Add(file);
        }

        _playlist.Clear();

        if (Globals.RandomizeMusic)
        {
            RandomizePlaylist();
        }
        else
        {
            foreach (var song in _songList)
                _playlist.Enqueue(song);
        }

        _curVol = Globals!.SettingsManager!.GetValue("music_volume", 1.0f);
        AudioServer.SetBusVolumeDb(_bus_index, Mathf.LinearToDb(_curVol));
        if (Globals.AutoPlayMusic)
            PlayerOnFinished();
    }

    private void OnSettingsUpdated()
    {
        var newFolders = new List<string>();
        foreach (var folder in Globals!.MusicFolders)
        {
            var found = false;
            foreach (var song in _songList)
            {
                if (song.StartsWith(folder))
                {
                    found = true;
                    break;
                }
            }
            if (!found) newFolders.Add(folder);
        }
        
        foreach (var file in newFolders.SelectMany(path => Directory.EnumerateFiles(path)))
        {
            if (file.EndsWith(".mp3")) _songList.Add(file);
            if (file.EndsWith(".ogg")) _songList.Add(file);
        }

        if (Globals.RandomizeMusic)
        {
            var wasPlaying = _player!.Playing;
            if (_player.Playing) _player.Stop();
            RandomizePlaylist();
            if (wasPlaying) PlayerOnFinished();
        }
    }

    private void RandomizePlaylist()
    {
        var rng = new RandomNumberGenerator();
        var list = new List<string>(_songList);
        while (list.Count > 0)
        {
            var i = rng.RandiRange(0, list.Count-1);
            _playlist.Enqueue(list[i]);
            list.RemoveAt(i);
        }
    }

    private void HandleElgato(string data)
    {
        switch (data)
        {
            case "music play":
                if (_player!.Stream == null || !_player.Playing)
                    PlayerOnFinished();
                break;
            case "music pause":
                _player!.StreamPaused = !_player.StreamPaused;
                EmitSignal(SignalName.PauseChanged);
                break;
            case "music stop":
                if (_player!.Playing)
                    _player.Stop();
                break;
            case "music vol_up":
                ChangeVolume(0.05f);
                break;
            case "music vol_down":
                ChangeVolume(-0.05f);
                break;
            case "music next":
                if (_player!.Playing)
                {
                    _player.Stop();
                    PlayerOnFinished();
                }
                break;
            case "music curr_song":
                GD.Print($"Current Song: {_currSong}");
                break;
        }
    }

    private void ChangeVolume(float value)
    {
        var newVol = Mathf.Clamp(_curVol + value, 0.0f, 1.0f);
        
        GD.Print($"Volume Change: {_curVol} -> {newVol} (Change: {value})");
        _curVol = newVol;
        
        Globals!.SettingsManager!.SetValue("music_volume", _curVol);
        Globals.SettingsManager.SaveSettings();
        
        AudioServer.SetBusVolumeDb(_bus_index, Mathf.LinearToDb(_curVol));
    }

    public record TagInfo(string Artist, string Title, string AlbumArt);
}