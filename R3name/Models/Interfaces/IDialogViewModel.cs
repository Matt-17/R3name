using System.Windows.Input;

namespace R3name.Models.Interfaces;

public interface IDialogViewModel
{
    public event DialogWindowResult DialogResultRequested;
    ICommand OkCommand { get; }
}