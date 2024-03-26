using Godot;
using System;
using System.IO;
using Godot.Sharp.Extras;

namespace TwitchWidgetsApp.Tools;

public partial class Tool : Control
{
	[NodePath] private RichTextLabel? _history;
	[NodePath] private Button? _run;
	[Export(PropertyHint.File)] public string? TemplateFile;
	[Export(PropertyHint.Dir)] public string? DirectoryScan;
	[Export(PropertyHint.Dir)] public string? DirectoryOutput;

	private SpriteFrames? _template;
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		_template = GD.Load<SpriteFrames>(TemplateFile);
		_run!.Pressed += RunOnPressed;
	}

	private void RunOnPressed()
	{
		var dir = DirAccess.Open(DirectoryScan);
		dir.IncludeHidden = false;
		var files = dir.GetFiles();
		var totalFrames = 0;
		var totalFiles = 0;
		var totalAnimations = 0;
		foreach (var file in files)
		{
			if (file.EndsWith(".import")) continue;
			_history!.AppendText("Creating SpriteFrames for " + file + "\n");
			var numFrames = 0;
			var numAnims = 0;
			var texture = GD.Load<Texture2D>(Path.Join(DirectoryScan,file));
			var newFrames = (SpriteFrames)_template!.Duplicate(true);
			foreach (var animation in newFrames.GetAnimationNames())
			{
				for (var i = 0; i < newFrames.GetFrameCount(animation); i++)
				{
					var atlas = newFrames.GetFrameTexture(animation, i) as AtlasTexture;
					atlas!.Atlas = texture;
					newFrames.SetFrame(animation, i, atlas);
					numFrames++;
					numAnims++;
					totalFrames++;
				}

				totalAnimations++;
			}


			ResourceSaver.Save(newFrames, Path.Join(DirectoryOutput, Path.GetFileNameWithoutExtension(file) + ".tres"));
			_history.AppendText($"Created {numFrames} frames with {numAnims} animations for {file}\n");
			totalFiles++;
		}
		_history!.AppendText($"Created {totalFrames} total frames for {totalAnimations} animations in {totalFiles} files\n");
	}
}
