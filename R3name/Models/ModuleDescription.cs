using System;
using System.Diagnostics;
using System.Reflection;
using R3name.Modules.Attributes;

namespace R3name.Models;

public class ModuleDescription
{
    private readonly Type _modificatorType;

    public string Title { get; set; }
    public string Description { get; set; }

    public ModuleDescription(Type modificatorType)
    {
        _modificatorType = modificatorType;

        var attribute = _modificatorType.GetCustomAttribute<ModificatorAttribute>();
        Debug.Assert(attribute != null);

        Title = attribute.Title;
        Description = attribute.Description;
    }

    public T CreateModuleInstance<T>()
    {
        return (T)Activator.CreateInstance(_modificatorType);
    }
}