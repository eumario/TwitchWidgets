using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Sharp.Extras;
using TwitchWidgets.Data.Models;

namespace TwitchWidgetsApp.Library.Managers;

public partial class SettingsManager : Node
{
    [Singleton] public Globals Globals;
    private Dictionary<string, Setting> _settings = new();

    public override void _Ready()
    {
        this.OnReady();
    }

    public void LoadSettings()
    {
        foreach (var setting in Globals.Database.Settings.ToList())
        {
            _settings[setting.Key] = setting;
        }
    }

    public bool HasSetting(string name) => _settings.ContainsKey(name);

    public int GetValue(string key, int def) => HasSetting(key) ? _settings[key].GetValue(def) : def;
    public float GetValue(string key, float def) => HasSetting(key) ? _settings[key].GetValue(def) : def;
    public bool GetValue(string key, bool def) => HasSetting(key) ? _settings[key].GetValue(def) : def;
    public string GetValue(string key, string def) => HasSetting(key) ? _settings[key].GetValue(def) : def;
    public double GetValue(string key, double def) => HasSetting(key) ? _settings[key].GetValue(def) : def;

    private void EnsureKey(string key)
    {
        if (!HasSetting(key))
        {
            _settings[key] = new Setting() { Key = key };
            Globals.Database.Settings.Add(_settings[key]);
        }
    }

    public void SetValue(string key, int val)
    {
        EnsureKey(key);
        _settings[key].SetValue(val);
    }

    public void SetValue(string key, float val)
    {
        EnsureKey(key);
        _settings[key].SetValue(val);
    }

    public void SetValue(string key, bool val)
    {
        EnsureKey(key);
        _settings[key].SetValue(val);
    }
    
    public void SetValue(string key, string val)
    {
        EnsureKey(key);
        _settings[key].SetValue(val);
    }
    
    public void SetValue(string key, double val)
    {
        EnsureKey(key);
        _settings[key].SetValue(val);
    }

    public void SaveSettings() => Globals.Database.SaveChanges();

    public void ResetSettings()
    {
        _settings.Clear();
        LoadSettings();
    }

}