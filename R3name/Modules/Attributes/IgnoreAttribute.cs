using System;

namespace R3name.Modules.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class IgnoreAttribute : Attribute
{
    public IgnoreAttribute()
    {
    }
}