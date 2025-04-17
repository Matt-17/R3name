using System;

namespace R3name.Modules.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ModificatorAttribute : Attribute
{
    public string Title { get; }
    public string Description { get; }

    public ModificatorAttribute(string title, string description)
    {
        Title = title;
        Description = description;
    }
}