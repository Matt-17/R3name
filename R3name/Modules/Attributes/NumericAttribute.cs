using System;

namespace R3name.Modules.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class NumericAttribute : Attribute
{
    public int Minimum { get; set; }
    public int Maximum { get; set; }
    public int Step { get; set; }

    public NumericAttribute()
    {
        Minimum = int.MinValue;
        Maximum = int.MaxValue;
        Step = 1;
    }
}