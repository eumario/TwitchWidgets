using System;
using System.Collections.Generic;
using System.Net;
using Godot;
using Godot.Sharp.Extras;
using HttpClient = System.Net.Http.HttpClient;

namespace TwitchWidgetsApp.Library.Managers;

public partial class TtsManager : Node
{
    [Singleton] public Globals Globals;
    [Signal] public delegate void TtsFinishedEventHandler(string msg);
    [Signal] public delegate void TtsDownloadedEventHandler(string msg);
    public List<string> Voices = [
        "Aditi", "Amy", "Astrid", "Bianca", "Brian", "Carla", "Carmen", "Celine",
        "Chantal", "Conchita", "Cristiano", "Dora", "Emma", "Enrique", "Ewa",
        "Filiz", "Geraint", "Giorgio", "Gwyneth", "Hans", "Ines", "Ivy", "Jacek", "Jan",
        "Joanna", "Joey", "Justin", "Karl", "Kendra", "Kimberly", "Liv", "Lotte",
        "Mads", "Maja", "Marlene", "Mathieu", "Matthew", "Maxim", "Mia", "Miguel",
        "Mizuki", "Naja", "Nicole", "Penelope", "Raveena", "Ricardo", "Ruben",
        "Russell", "Salli", "Seoyeon", "Takumi", "Tatyana", "Vicki", "Vitoria", "Zhiyu"
    ];
    public string? SelectedVoice = null;
    private List<TextToSpeech> _played = [];
    private Queue<TextToSpeech> _queued = new();
    private AudioStreamPlayer _player;
    private HttpClient _httpClient = new();
    public int TotalQueue => _played.Count + _queued.Count;
    public int CurrentQueue => _played.Count;
    public override void _Ready()
    {
        this.OnReady();
        _player = new AudioStreamPlayer();
        _player.Bus = "tts";
        _player.Finished += TtsPlayerOnFinished;
        AddChild(_player);
    }

    public override void _ExitTree()
    {
        foreach(var tts in _played) tts.Free();
        while (_queued.Count > 0)
        {
            var tts = _queued.Dequeue();
            tts.Free();
        }
    }

    private async void FetchTTSMessage(string message)
    {
        var text = Uri.EscapeDataString(message);
        var json = await _httpClient.GetAsync(
            $"http://api.streamelements.com/kappa/v2/speech?voice={SelectedVoice}&text={text}");

        if (json.StatusCode != HttpStatusCode.OK)
        {
            OS.Alert("Bad Status Code? Voice = " + SelectedVoice + "\n" + json.StatusCode.ToString() + "\nRequest: " + json.ToString());
            return;
        }

        var data = await json.Content.ReadAsByteArrayAsync();
        var stream = new AudioStreamMP3();
        stream.Data = data;
        var tts = new TextToSpeech()
        {
            Message = message,
            Sound = stream
        };
        _queued.Enqueue(tts);
        EmitSignal(SignalName.TtsDownloaded, message);
    }

    private void TtsPlayerOnFinished()
    {
        EmitSignal(SignalName.TtsFinished, _played[^1]);
        _player.Stream = null;
    }

    public void AddTtsMessage(string message) => FetchTTSMessage(message);

    public void PlayNextMessage()
    {
        if (_player.Stream != null) return;
        if (_player.Playing) return;
        var tts = _queued.Dequeue();
        _played.Add(tts);
        _player.Stream = tts.Sound;
        _player.Play();
    }

    public void ClearAll()
    {
        if (_player.Stream != null) { Globals.RunOnMain(ClearAll); return; }
        if (_player.Playing) { Globals.RunOnMain(ClearAll); return; }

        foreach (var tts in _played) tts.Free();
        _played.Clear();
        while (_queued.Count != 0)
        {
            var tts = _queued.Dequeue();
            tts.Free();
        }
    }
}