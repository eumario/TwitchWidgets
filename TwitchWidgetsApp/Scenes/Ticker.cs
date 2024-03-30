using Godot;
using System.Linq;
using Godot.Sharp.Extras;
using TwitchWidgets.Data.Models;
using Globals = TwitchWidgetsApp.Library.Globals;

namespace TwitchWidgetsApp.Scenes;

public partial class Ticker : MarginContainer
{
	[Singleton] public Globals? Globals;
	[NodePath] private LineEdit? _message;
	[NodePath] private Button? _add;
	[NodePath] private ItemList? _tickerMessages;
	[NodePath] private Button? _edit;
	[NodePath] private Button? _remove;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		foreach (var message in Globals!.Database!.TickerMessages!.ToList())
		{
			var i = _tickerMessages!.ItemCount;
			_tickerMessages.AddItem(message.Message);
			_tickerMessages.SetItemMetadata(i, message.Id);
		}

		_add!.Pressed += AddOnPressed;
		_edit!.Pressed += EditOnPressed;
		_remove!.Pressed += RemoveOnPressed;

		_tickerMessages!.ItemSelected += index =>
		{
			var item = _tickerMessages.GetItemText((int)index);
			_message!.Text = item;
		};
	}

	private async void AddOnPressed()
	{
		var msg = _message!.Text;
		if (string.IsNullOrEmpty(msg))
		{
			OS.Alert("You need to provide a Message to add to the Ticker", "Add Ticker Message Error");
			return;
		}

		var tm = new TickerMessage() { Message = msg };
		Globals!.Database!.TickerMessages!.Add(tm);
		await Globals.Database.SaveChangesAsync();
		var i = _tickerMessages!.ItemCount;
		_tickerMessages.AddItem(msg);
		_tickerMessages.SetItemMetadata(i, tm.Id);
		_message.Text = "";
		Globals.EmitSignal(Globals.SignalName.UpdateTicker);
	}
	private async void EditOnPressed()
	{
		var i = _tickerMessages!.GetSelectedItems();
		if (i.Length == 0)
		{
			OS.Alert("You need to select a message to edit, before editing it.", "Edit Ticker Message Error");
			return;
		}

		var id = _tickerMessages.GetItemMetadata(i[0]).AsInt32();
		var tm = Globals!.Database!.TickerMessages!.FirstOrDefault(x => x.Id == id);
		if (tm == null)
		{
			OS.Alert("The Ticker Message you selected does not exist.", "Edit Ticker Message Error");
			return;
		}

		tm.Message = _message!.Text;
		await Globals.Database.SaveChangesAsync();
		_tickerMessages.SetItemText(i[0], _message.Text);
		_message.Text = "";
		_tickerMessages.Select(-1);
		Globals.EmitSignal(Globals.SignalName.UpdateTicker);
	}

	private void RemoveOnPressed()
	{
		var i = _tickerMessages!.GetSelectedItems();
		if (i.Length == 0)
		{
			OS.Alert("You need to select a message to remove, before removing it.", "Remove Ticker Message Error");
			return;
		}

		var dlg = new ConfirmationDialog()
		{
			DialogAutowrap = true,
			DialogCloseOnEscape = false,
			DialogHideOnOk = false,
			OkButtonText = "Yes",
			CancelButtonText = "No",
			Title = "Remove Ticker Message",
			DialogText = $"Are you sure you want to delete the Ticker Message '{_tickerMessages.GetItemText(i[0])}'?"
		};
		dlg.Confirmed += () =>
		{
			var id = _tickerMessages.GetItemMetadata(i[0]).AsInt32();
			var tm = Globals!.Database!.TickerMessages!.FirstOrDefault(x => x.Id == id);
			Globals!.Database!.TickerMessages!.Remove(tm!);
			_tickerMessages.RemoveItem(i[0]);
			_message!.Text = "";
			_tickerMessages.Select(-1);
			Globals.Database.SaveChanges();
			dlg.QueueFree();
			Globals.EmitSignal(Globals.SignalName.UpdateTicker);
		};
		dlg.Canceled += () =>
		{
			dlg.QueueFree();
		};
		dlg.CloseRequested += () =>
		{
			if (IsInstanceValid(dlg))
				dlg.QueueFree();
		};
		AddChild(dlg);
		dlg.PopupCentered(new Vector2I(300, 200));
	}
}
