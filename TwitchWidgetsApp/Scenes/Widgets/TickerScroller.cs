using System.Linq;
using Godot;
using Godot.Collections;
using Godot.Sharp.Extras;
using Globals = TwitchWidgetsApp.Library.Globals;

namespace TwitchWidgetsApp.Scenes.Widgets;

[Tool]
[GlobalClass]
public partial class TickerScroller : Control
{
    [Singleton] public Globals? Globals;
    [ExportCategory("Message Settings")]
    [Export] public Array<string> Messages = [];
    [Export] public LabelSettings LabelSettings = new();
    [ExportCategory("Speed and Spacing")]
    [Export] public double Speed = 30.0;
    [Export] public double Spacing = 40.0;
    [Export] public string SeparatorText = "";

    #region Godot Overrides
    public override void _Ready()
    {
        this.OnReady();
        ClearMessages();
        Globals?.Database?.TickerMessages?.ToList().ForEach(x => AddTicker(x.Message));
        ClipContents = true;
        ResetTicker();

        Globals!.UpdateTicker += () =>
        {
            ClearMessages();
            Globals?.Database?.TickerMessages?.ToList().ForEach(x => AddTicker(x.Message));
        };
    }

    public override void _Process(double delta)
    {
        foreach (var node in GetChildren())
        {
            var lbl = node as Label;
            lbl!.Position = new Vector2(lbl.Position.X - (float)(Speed * delta), 0);
        }

        foreach (var node in GetChildren())
        {
            var lbl = node as Label;
            if (lbl!.Position.X < -lbl.Size.X)
                ResetPosition(lbl);
        }
    }
    #endregion
    
    #region Public API
    public void AddTicker(string msg) { Messages.Add(msg); ResetTicker(); }
    public void RemoveMessage(int index) { Messages.RemoveAt(index); ResetTicker(); }

    public void ClearMessages()
    {
        Messages.Clear();
        foreach (var node in GetChildren()) node.QueueFree();
    }

    public void ResetPosition(Label lbl)
    {
        var sx = Size.X;
        foreach (var node in GetChildren())
        {
            var clbl = node as Label;
            if (clbl!.Position.X + clbl.Size.X > sx)
                sx = clbl.Position.X + clbl.Size.X + (float)Spacing;
        }

        lbl.Position = new Vector2(sx, 0);
    }

    private float MakeLabel(string msg, float startX)
    {
        var lbl = new Label
        {
            Text = msg,
            LabelSettings = LabelSettings,
            Position = new Vector2(startX, 0)
        };
        AddChild(lbl);
        if (Size.Y < lbl.Size.Y)
        {
            Size = new Vector2(Size.X, lbl.Size.Y);
            CustomMinimumSize = Size;
        }
        startX += lbl.Size.X + (float)Spacing;
        return startX;
    }

    public void ResetTicker()
    {
        foreach (var node in GetChildren()) node.QueueFree();

        var sx = Size.X;
        foreach (var msg in Messages)
        {
            sx = MakeLabel(msg, sx);
            if (SeparatorText != "")
            {
                sx = MakeLabel(SeparatorText, sx);
            }
        }
    }
    #endregion
}
