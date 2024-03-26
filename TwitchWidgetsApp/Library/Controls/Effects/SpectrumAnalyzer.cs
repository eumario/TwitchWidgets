using Godot;
using Godot.Sharp.Extras;

namespace TwitchWidgetsApp.Library.Controls.Effects;
public partial class SpectrumAnalyzer : ColorRect
{
	[Singleton] public Globals? Globals;
	private ShaderMaterial? SpectrumShader = null;
	private AudioEffectSpectrumAnalyzerInstance? Analyzer = null;

	private const int VU_COUNT = 32;
	private const float FREQ_MAX = 3000.0f;
	private const float MIN_DB = 80.0f;
	private bool cleared = false;
	public override void _Ready()
	{
		this.OnReady();
		SpectrumShader = (ShaderMaterial)this.Material;
		Analyzer = Globals!.MusicController!.GetSpectrum();
	}
	
	// func ConvertLinearToDB(volume):
	//	var returnValue = log(volume) * 8.6858896380650365530225783783321
	// 	return(returnValue)
	double Linear2DB(double vol)
	{
		return Mathf.Log((float)vol) * 8.6858896380650365530225783783321;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Analyzer is null) return;
		if (!Globals!.MusicController!.IsPlaying)
		{
			if (cleared) return;
			cleared = true;
			for (int i = 0; i < VU_COUNT; i++)
			{
				double hz = (i + 1) * FREQ_MAX / VU_COUNT;
				SpectrumShader!.SetShaderParameter($"hz{i}", 0.0);
			}
	
			return;
		}

		cleared = false;
		var prevHz = 0.0;
		for (var i = 0; i < VU_COUNT; i++)
		{
			var hz = (i + 1) * FREQ_MAX / VU_COUNT;
			var mag = Analyzer.GetMagnitudeForFrequencyRange((float)prevHz, (float)hz).Length();
			SpectrumShader!.SetShaderParameter($"hz{i}", (float)Mathf.Clamp(((float)Linear2DB(mag) + MIN_DB)/MIN_DB, 0.0f, 1.0f));
			prevHz = hz;
		}
	}
}
