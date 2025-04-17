using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using R3name.Helper;

namespace R3name.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    public virtual event PropertyChangedEventHandler PropertyChanged;


    protected ICommand Command(Action action, bool canExecute = true)
    {
        var command = new RelayCommand(action, canExecute);
        _commands.Add(command);
        return command;
    }
    protected ICommand Command(Action action, Func<bool> canExecute)
    {
        var command = new RelayCommand(action, canExecute);
        _commands.Add(command);
        return command;
    }

    protected ICommand Command<T>(Action<T> action, bool canExecute = true)
    {
        var command = new RelayCommand<T>(action, canExecute);
        _commands.Add(command);
        return command;
    }
    protected ICommand Command<T>(Action<T> action, Func<bool> canExecute)
    {
        var command = new RelayCommand<T>(action, canExecute);
        _commands.Add(command);
        return command;
    }
    protected ICommand Command<T>(Action<T> action, Func<T, bool> canExecute)
    {
        var command = new RelayCommand<T>(action, canExecute);
        _commands.Add(command);
        return command;
    }
    private readonly List<ICommand> _commands = new List<ICommand>();

    protected void UpdateCommands()
    {
        foreach (var command in _commands)
        {
            command.Update();
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}