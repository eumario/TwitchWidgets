using System;
using Godot;
using Godot.Sharp.Extras;
using OBSStudioClient.Messages;
using OBSStudioClient.Responses;

namespace TwitchWidgetsApp.Library.Controls;

public partial class ObsStatus : PanelContainer
{
	[Singleton] public Globals? Globals;
	[NodePath] private Label? _obsConnected;
	[NodePath] private Label? _streaming;
	[NodePath] private Label? _streamStats;
	[NodePath] private Label? _recording;
	[NodePath] private Label? _recordStats;
	private Timer? Timer;
	
	public override void _Ready()
	{
		this.OnReady();
		Timer = new Timer();
		Timer.Autostart = true;
		Timer.OneShot = false;
		Timer.WaitTime = 1.0;
		Timer.Timeout += TimerOnTimeout;
		AddChild(Timer);
		_obsConnected!.Text = "Offline";
		_obsConnected.SelfModulate = Colors.Red;

		_streaming!.Text = "Offline";
		_streaming.SelfModulate = Colors.Red;

		_recording!.Text = "Not Recording";
		_recording.SelfModulate = Colors.Goldenrod;
	}

	private async void TimerOnTimeout()
	{
		if (Globals!.ObsManager!.IsConnected())
		{
			_obsConnected!.Text = "Online";
			_obsConnected.SelfModulate = Colors.Green;
			var batchRequest = new RequestBatchMessage();
			batchRequest.AddGetStreamStatusRequest();
			batchRequest.AddGetRecordStatusRequest();
			var results = await Globals!.ObsManager!.Client!.SendRequestBatchAsync(batchRequest);
			foreach (var result in results)
			{
				switch (result.ResponseData)
				{
					case RecordStatusResponse recordStatus:
						_recording!.Text = recordStatus.OutputActive ? "Recording" : "Not Recording";
						_recording.SelfModulate = recordStatus.OutputActive ? Colors.Green : Colors.Goldenrod;
						var recordTime = TimeSpan.FromMilliseconds(recordStatus.OutputDuration);
						var recordBytes = Util.FormatSize(recordStatus.OutputBytes);
						_recordStats!.Text = $"{recordTime:g} {recordBytes}";
						break;
					case OutputStatusResponse streamStatus:
						_streaming!.Text = streamStatus.OutputActive ? "Online" : "Offline";
						_streaming.SelfModulate = streamStatus.OutputActive ? Colors.Green : Colors.Red;
						var streamTime = TimeSpan.FromMilliseconds(streamStatus.OutputDuration);
						var streamBytes = Util.FormatSize(streamStatus.OutputBytes);
						var frameCount = $"{streamStatus.OutputSkippedFrames}/{streamStatus.OutputTotalFrames}";
						_streamStats!.Text = $"{streamTime:g} {streamBytes} ({frameCount})";
						break;
				}
			}
		}
		else
		{
			_obsConnected!.Text = "Offline";
			_obsConnected.SelfModulate = Colors.Red;

			_streaming!.Text = "Offline";
			_streaming.SelfModulate = Colors.Red;

			_recording!.Text = "Not Recording";
			_recording.SelfModulate = Colors.Goldenrod;
		}
	}
}
