using Godot;
using Godot.Sharp.Extras;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TwitchWidgets.Data.Models;
using TwitchWidgetsApp.Library;

public partial class Heckle : MarginContainer
{
	[Singleton] public Globals? Globals = null;
	[NodePath] private Button? _approve = null;
	[NodePath] private Button? _reject = null;
	[NodePath] private LineEdit? _customHeckle = null;
	[NodePath] private Button? _addHeckle = null;
	[NodePath] private Tree? _heckles = null;
	private TreeItem? _root = null;
	private HeckleMessage? _currentSelectedHeckle = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.OnReady();
		_root = _heckles!.CreateItem();
		_heckles.SetColumnTitle(0, "ID");
		_heckles.SetColumnTitle(1, "Submitted By");
		_heckles.SetColumnTitle(2, "Heckle");
		_heckles.SetColumnTitle(3, "Submitted On");
		_heckles.SetColumnTitle(4, "Approved On");

		_heckles.SetColumnExpand(0, false);
		_heckles.SetColumnExpand(1, false);
		_heckles.SetColumnExpand(3, false);
		_heckles.SetColumnExpand(4, false);
		_heckles.SetColumnClipContent(2,true);
		foreach(var heckle in Globals!.Database!.HeckleMessages!.Include(x => x.SuggestedBy!))
        {
            AddHeckleItem(heckle);
        }
        _approve!.Disabled = true;
		_reject!.Disabled = true;
		_heckles.ItemSelected += HandleHeckleSelected;
		_approve!.Pressed += HandleApprove;
		_reject!.Pressed += HandleReject;
		_addHeckle!.Pressed += HandleAddHeckle;
	}

    private void AddHeckleItem(HeckleMessage heckle)
    {
        var user = heckle.SuggestedBy!;
        var hItem = _heckles!.CreateItem(_root);
        hItem.SetText(0, heckle.Id.ToString());
        hItem.SetText(1, user!.DisplayName);
        hItem.SetText(2, heckle.Heckle);
        hItem.SetText(3, $"{heckle.SuggestedAt:MM/dd/yy}");
        if (heckle.Approved)
            hItem.SetText(4, $"{heckle.ApprovedAt:MM/dd/yy}");
        else if (heckle.Rejected)
            hItem.SetText(4, "Rejected");
        else
            hItem.SetText(4, "Not Yet Approved.");
    }

	private void UpdateHeckleItem(TreeItem hItem) 
	{
		var user = _currentSelectedHeckle!.SuggestedBy;
		hItem.SetText(0, _currentSelectedHeckle.Id.ToString());
        hItem.SetText(1, user!.DisplayName);
        hItem.SetText(2, _currentSelectedHeckle!.Heckle);
        hItem.SetText(3, $"{_currentSelectedHeckle!.SuggestedAt:MM/dd/yy}");
        if (_currentSelectedHeckle!.Approved)
            hItem.SetText(4, $"{_currentSelectedHeckle!.ApprovedAt:MM/dd/yy}");
        else if (_currentSelectedHeckle!.Rejected)
            hItem.SetText(4, "Rejected");
        else
            hItem.SetText(4, "Not Yet Approved.");
	}

    private void HandleHeckleSelected()
	{
		var hItem = _heckles!.GetSelected();
		var id = hItem.GetText(0).ToInt();
		var heckle = Globals!.Database!.HeckleMessages!.Find(id);
		if (heckle == null) return;
		_currentSelectedHeckle = heckle;
		_approve!.Disabled = false;
		_reject!.Disabled = false;
	}

	private async void HandleApprove() 
	{
		_currentSelectedHeckle!.Approved = true;
		_currentSelectedHeckle.Rejected = false;
		_currentSelectedHeckle.ApprovedAt = DateTime.Today;
		await Globals!.Database!.SaveChangesAsync();
		UpdateHeckleItem(_heckles!.GetSelected());
		ResetTree();
	}

	private async void HandleReject()
	{
		_currentSelectedHeckle!.Approved = false;
		_currentSelectedHeckle.Rejected = true;
		_currentSelectedHeckle.ApprovedAt = DateTime.Today;
		await Globals!.Database!.SaveChangesAsync();
		UpdateHeckleItem(_heckles!.GetSelected());
		ResetTree();
	}

	private async void HandleAddHeckle()
	{
		var user = Globals!.Database!.KnownChatters!.FirstOrDefault(x => x.TwitchId == Globals!.Streamer!.id);
		if (user == null) {
			OS.Alert("Unable to find Streamer Known Chatter.", "Add Custom Heckle");
			return;
		}
		var msg = new HeckleMessage() {
			Heckle = _customHeckle!.Text,
			SuggestedBy = user,
			Approved = true,
			Rejected = false,
			SuggestedAt = DateTime.Today,
			ApprovedAt = DateTime.Today
		};
		Globals!.Database!.HeckleMessages!.Add(msg);
		await Globals!.Database!.SaveChangesAsync();
		AddHeckleItem(msg);
		_customHeckle.Text = "";
	}

	private void ResetTree()
	{
		_currentSelectedHeckle = null;
		_heckles!.DeselectAll();
		_approve!.Disabled = true;
		_reject!.Disabled = true;
	}
}
