using System.Windows.Input;

namespace R3name.Helper;

public static class RelayCommandExtension
{
    public static void Update(this ICommand command)
    {
        if (command is RelayCommand rc)
            rc.UpdateCanExecute();
    }
}