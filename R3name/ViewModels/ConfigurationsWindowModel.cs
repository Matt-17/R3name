using R3name.Models.Interfaces;
using System.Windows.Input;
using R3name.Models;

namespace R3name.ViewModels;

internal class ConfigurationsWindowModel : BaseViewModel, IDialogViewModel
{
    public ConfigurationsWindowModel()
    {

        OkCommand = Command(Ok, () => true);
    }
    public ICommand OkCommand { get; }
    private void Ok()
    {
        OnDialogCloseRequested(true);
    }
    public event DialogWindowResult DialogResultRequested;

    protected virtual void OnDialogCloseRequested(bool dialogresult)
    {
        DialogResultRequested?.Invoke(dialogresult);
    }
}