using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using R3name.Modules.Attributes;

namespace R3name.Helper;

static class AssemblyHelper
{
    public static void SetTemplate(Type dataType, Type templateType)
    {
        var dataTemplateKey = new DataTemplateKey(dataType);
        var template = new DataTemplate
        {
            VisualTree = new FrameworkElementFactory(templateType)
        };
        Application.Current.Resources.Add(dataTemplateKey, template);
    }

    public static void Compose()
    {
        ComposeViews();
    }
    private static void ComposeViews()
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null)
            throw new NullReferenceException();
        var types = assembly.GetTypes();

        var typeDict = new Dictionary<string, Type>();
        foreach (var type in types)
        {
            if (type.Name.EndsWith("View"))
                typeDict.Add(type.Name, type);
        }

        foreach (var modType in types)
        {
            if (modType.GetCustomAttribute<ModificatorAttribute>() == null)
                //throw new InvalidOperationException($"Class {modType.Name} is not decorated with [{nameof(ModificatorAttribute)}(string title, string description)].");
                continue;

            if (!typeDict.TryGetValue(modType.Name + "View", out var viewType))
                //throw new NullReferenceException($"Type {modType.Name}View does not exist.");
                continue;

            SetTemplate(modType, viewType);
        }
    }

    public static IEnumerable<Type> GetModules<T>()
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null)
            throw new NullReferenceException();
        var types = assembly.GetTypes();
        var desiredType = typeof(T);
        foreach (var type in types)
        {
            if (!type.IsSubclassOf(desiredType))
                continue;

            var attribute = type.GetCustomAttribute<ModificatorAttribute>();
            if (attribute == null)
                continue;

            yield return type;
        }
    }
}