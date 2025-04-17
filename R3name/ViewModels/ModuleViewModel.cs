using System;
using System.Reflection;
using System.Text.Json;
using System.Windows.Input;

using R3name.Modules;
using R3name.Modules.Attributes;
using R3name.Modules.Modificators;

namespace R3name.ViewModels;

public sealed class ModuleViewModel : BaseViewModel
{
    private bool _isDeactivated;

    private ModuleViewModel(object modificator)
    {
        Module = modificator;
        ActivateCommand = Command(() => IsDeactivated = false, () => IsDeactivated);
        DeactivateCommand = Command(() => IsDeactivated = true, () => !IsDeactivated);
        ResetCommand = Command(Reset);
        RemoveCommand = Command(Remove);
        DuplicateCommand = Command(Duplicate);
    }

    public bool IsDeactivated
    {
        get
        {
            if (Module is Modificator modificator)
                return modificator.IsDeactivated;
            return _isDeactivated;
        }
        set
        {
            if (Module is Modificator modificator)
            {
                modificator.IsDeactivated = value;
                OnPropertyChanged();
                UpdateCommands();
                RefreshParent();
                return;
            }

            if (value == _isDeactivated)
                return;
            _isDeactivated = value;
            OnPropertyChanged();
            UpdateCommands();
            RefreshParent();
        }
    }

    public ICommand DuplicateCommand { get; }

    public ICommand RemoveCommand { get; }

    public ICommand ResetCommand { get; }

    public ICommand ActivateCommand { get; }

    public ICommand DeactivateCommand { get; }

    public object Module { get; private set; }

    public MainViewModel Parent { get; private init; }

    public static ModuleViewModel Create(MainViewModel parent, Processor module)
    {
        var vm = new ModuleViewModel(module)
        {
            Parent = parent
        };
        return vm;
    }

    public static ModuleViewModel Create(MainViewModel parent, Modificator module)
    {
        var vm = new ModuleViewModel(module)
        {
            Parent = parent
        };
        return vm;
    }

    public string Title => Module.GetType()
        .GetCustomAttribute<ModificatorAttribute>()?
        .Title;

    private void RefreshParent() => Parent?.Refresh();

    private void Reset()
    {
        _isDeactivated = false;
        var type = Module.GetType();
        Module = Activator.CreateInstance(type);
        OnPropertyChanged(nameof(Module));
        OnPropertyChanged(nameof(IsDeactivated));
        UpdateCommands();
        RefreshParent();
    }

    private void Duplicate()
    {
        var serializedModule = JsonSerializer.Serialize(Module);

        var copy = JsonSerializer.Deserialize(serializedModule, Module.GetType());
        var vm = new ModuleViewModel(copy)
        {
            Parent = Parent
        };

        var index = Parent.Modificators.IndexOf(this);
        Parent.Modificators.Insert(index + 1, vm);
        RefreshParent();
    }

    private void Remove()
    {
        switch (Module)
        {
            case Modificator:
                Parent.Modificators.Remove(this);
                break;
            case Processor:
                Parent.FileProcessors.Remove(this);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        RefreshParent();
    }
}