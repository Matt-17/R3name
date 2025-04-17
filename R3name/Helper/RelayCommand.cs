using System;
using System.Windows.Input;

namespace R3name.Helper;

public class RelayCommand : ICommand
{
    private readonly Action<object> _action;
    private readonly Func<object, bool> _canExecuteFunc;
    private readonly bool _canExecute;

    public RelayCommand(Action action, bool canExecute = true) : this(_ => action(), canExecute)
    { }

    public RelayCommand(Action action, Func<bool> canExecute) : this(_ => action(), _ => canExecute())
    { }


    protected RelayCommand(Action<object> action, bool canExecute)
    {
        _action = action;
        _canExecute = canExecute;
    }

    protected RelayCommand(Action<object> action, Func<object, bool> canExecute)
    {
        _action = action;
        _canExecuteFunc = canExecute;
    }

    public void UpdateCanExecute()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool CanExecute(object parameter)
    {
        try
        {
            return _canExecuteFunc?.Invoke(parameter) ?? _canExecute;
        }
        catch
        {
            return false;
        }
    }

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
        _action(parameter);
    }
}
public class RelayCommand<T> : RelayCommand
{
    public RelayCommand(Action<T> action, bool canExecute = true) : base(o => action((T)o), canExecute)
    {
    }

    public RelayCommand(Action<T> action, Func<bool> canExecute) : base(o => action((T)o), _ => canExecute())
    {
    }

    public RelayCommand(Action<T> action, Func<T, bool> canExecute) : base(o => action((T)o), o => canExecute((T)o))
    {
    }
}