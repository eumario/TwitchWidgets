using System.ComponentModel.DataAnnotations.Schema;

namespace TwitchWidgets.Data.Models;

public class Setting
{
    public int Id { get; set; }
    
    public string Key { get; set; } = String.Empty;
    
    public string? SValue { get; set; }
    
    public bool? BValue { get; set; }
    
    public int? IValue { get; set; }
    
    public float? FValue { get; set; }
    
    public double? DValue { get; set; }

    public int GetValue(int def) => IValue ?? def;
    public float GetValue(float def) => FValue ?? def;
    public bool GetValue(bool def) => BValue ?? def;
    public string GetValue(string def) => SValue ?? def;
    public double GetValue(double def) => DValue ?? def;

    public void SetValue(int value) => IValue = value;
    public void SetValue(float value) => FValue = value;
    public void SetValue(bool value) => BValue = value;
    public void SetValue(string value) => SValue = value;
    public void SetValue(double value) => DValue = value;
}