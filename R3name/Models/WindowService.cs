using System.Windows;
using R3name.Models.Interfaces;
using R3name.ViewModels;
using R3name.Views;

namespace R3name.Models;

class WindowService : IWindowService
{
    private readonly Window _owner;
    public static WindowService Instance { get; private set; }


    private WindowService(Window owner)
    {
        _owner = owner;
    }

    public static void CreateInstance(Window owner)
    {
        Instance = new WindowService(owner);
    }

    public bool ShowSelectModuleDialog(SelectModuleViewModel vm)
    {
        var window = new SelectModuleWindow
        {
            DataContext = vm,
            Owner = _owner
        };

        if (window.ShowDialog() != true)
            return false;

        return true;
    }
}