using Godot;
using Godot.Sharp.Extras;
using TwitchWidgets.Data.Models;
using Globals = TwitchWidgetsApp.Library.Globals;

namespace TwitchWidgetsApp.Scenes;

public partial class TextCommands : MarginContainer
{
	[Singleton] public Globals? Globals;
	[NodePath] private LineEdit? _commandName;
	[NodePath] private LineEdit? _commandAlias;
	[NodePath] private LineEdit? _commandHelp;
	[NodePath] private LineEdit? _commandDescription;
	[NodePath] private Tree? _commandList;
	[NodePath] private Button? _addCommand;
	[NodePath] private Button? _updateCommand;
	[NodePath] private Button? _deleteCommand;

	private TreeItem? _rootNode;

	private TextCommand? _selectedCommand;

	public override void _Ready()
	{
		this.OnReady();
		_rootNode = _commandList!.CreateItem();
		_commandList.SetColumnTitle(0, "Command");
		_commandList.SetColumnTitle(1, "Alias");
		_commandList.SetColumnTitle(2, "Note");
		_commandList.SetColumnTitle(3, "Message");
		LoadCommands();

		_addCommand!.Pressed += AddCommandOnPressed;
		_updateCommand!.Pressed += UpdateCommandOnPressed;
		_deleteCommand!.Pressed += DeleteCommandOnPressed;
		_commandList.ItemSelected += CommandListOnItemSelected;
		ResetFields();
	}

	private void CommandListOnItemSelected()
	{
		var item = _commandList!.GetSelected();
		var id = item.GetMetadata(0).AsInt32();
		var cmd = Globals!.Database!.Commands!.Find(id);
		if (cmd == null) return;
		_selectedCommand = cmd;
		_updateCommand!.Disabled = false;
		_deleteCommand!.Disabled = false;
		_commandName!.Text = cmd.CommandName;
		_commandAlias!.Text = cmd.CommandAlias;
		_commandHelp!.Text = cmd.CommandHelp;
		_commandDescription!.Text = cmd.CommandDescription;
	}

	private void AddCommandOnPressed()
	{
		if (_commandName!.Text == "" || _commandHelp!.Text == "" ||
			_commandDescription!.Text == "")
		{
			OS.Alert("Please make sure to fill in all information for info text command.", "Add Command Failed");
			return;
		}

		var cmd = new TextCommand()
		{
			CommandName = _commandName.Text,
			CommandAlias = _commandAlias!.Text,
			CommandHelp = _commandHelp.Text,
			CommandDescription = _commandDescription.Text
		};
		ResetFields();
		Globals!.Database!.Commands!.Add(cmd);
		Globals.Database.SaveChanges();
		ResetList();
		Globals.EmitSignal(Globals.SignalName.UpdateCommands);
	}

	private void ResetList()
	{
		_commandList!.Clear();
		_rootNode = _commandList.CreateItem();
		LoadCommands();
	}

	private void ResetFields()
	{
		_commandName!.Text = "";
		_commandAlias!.Text = "";
		_commandHelp!.Text = "";
		_commandDescription!.Text = "";
		_updateCommand!.Disabled = true;
		_deleteCommand!.Disabled = true;
	}

	private void UpdateCommandOnPressed()
	{
		_selectedCommand!.CommandName = _commandName!.Text;
		_selectedCommand.CommandAlias = _commandAlias!.Text;
		_selectedCommand.CommandHelp = _commandHelp!.Text;
		_selectedCommand.CommandDescription = _commandDescription!.Text;
		Globals!.Database!.SaveChanges();
		ResetFields();
		ResetList();
		Globals.EmitSignal(Globals.SignalName.UpdateCommands);
	}

	private void DeleteCommandOnPressed()
	{
		var dlg = new ConfirmationDialog
		{
			DialogAutowrap = true,
			DialogCloseOnEscape = false,
			DialogHideOnOk = false,
			OkButtonText = "Yes",
			CancelButtonText = "No",
			DialogText = $"Are you sure you want to delete '{_selectedCommand!.CommandName}'?",
			Title = "Remove Command"
		};
		dlg.Confirmed += () =>
		{
			Globals!.Database!.Commands!.Remove(_selectedCommand);
			Globals.Database.SaveChanges();
			ResetFields();
			ResetList();
			dlg.QueueFree();
			Globals.EmitSignal(Globals.SignalName.UpdateCommands);
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

	private void LoadCommands()
	{
		if (Globals!.Database!.Commands == null) { Globals.RunOnMain(LoadCommands); return; }

		foreach (var command in Globals.Database.Commands)
		{
			var item = _commandList!.CreateItem(_rootNode);
			item.SetText(0, command.CommandName);
			item.SetText(1, command.CommandAlias);
			item.SetText(2, command.CommandHelp);
			item.SetText(3, command.CommandDescription);
			item.SetMetadata(0, command.Id);
		}
	}
}
