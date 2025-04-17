using System;

namespace R3name.Modules.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class UseTwoLinesAttribute : Attribute
{

    public UseTwoLinesAttribute()
    {
    }
}