using System;

namespace R3name.Models.Enums;

internal class EnumWrapper
{
    public Enum Value { get; }
    public string Name { get; }

    public EnumWrapper(Enum value, string name)
    {
        Value = value;
        Name = name;
    }
}