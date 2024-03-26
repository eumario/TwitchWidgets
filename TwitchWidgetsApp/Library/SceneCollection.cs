using Godot;
using System;

namespace TwitchWidgetsApp.Library;

[Tool]
[GlobalClass]
public partial class SceneCollection : Resource
{
    [ExportCategory("Collection Information")]
    [Export] public string? CollectionName;

    [ExportCategory("OBS Information")]
    [Export] public string? StartingSoonName;
    [Export] public string? TalkingName;
    [Export] public string? BeRightBackName;
    [Export] public string? DesktopName;
    [Export] public string? GamingName;
    [Export] public string? StreamEndingName;
    
    [ExportCategory("Scene Collection")]
    [Export] public PackedScene? StartingSoon;
    [Export] public PackedScene? Talking;
    [Export] public PackedScene? BRB;
    [Export] public PackedScene? Desktop;
    [Export] public PackedScene? Gaming;
    [Export] public PackedScene? StreamEnding;

    public PackedScene? this[string key]
    {
        get
        {
            if (key == StartingSoonName) return StartingSoon;
            if (key == TalkingName) return Talking;
            if (key == BeRightBackName) return BRB;
            if (key == DesktopName) return Desktop;
            if (key == GamingName) return Gaming;
            return key == StreamEndingName ? StreamEnding : null;
        }
    }
}
