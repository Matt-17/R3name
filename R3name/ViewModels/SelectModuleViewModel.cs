using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using R3name.Helper;
using R3name.Models;
using R3name.Models.Interfaces;

namespace R3name.ViewModels;

public class SelectModuleViewModel : BaseViewModel, IDialogViewModel
{
    private ModuleDescription _selected;

    public SelectModuleViewModel()
    {
        Modules = new List<ModuleGroup>();
        OkCommand = Command(Ok, () => Selected != null);
    }

    public ICommand OkCommand { get; }

    private void Ok()
    {
        OnDialogCloseRequested(true);
    }

    public List<ModuleGroup> Modules { get; }

    public ModuleDescription Selected
    {
        get => _selected;
        set
        {
            if (Equals(value, _selected)) return;
            _selected = null;
            OnPropertyChanged();
            _selected = value;
            OnPropertyChanged();
            UpdateCommands();
        }
    }

    public void LoadModules<T>(string header)
    {
        var list = AssemblyHelper.GetModules<T>()
            .Select(x => new ModuleDescription(x))
            .OrderBy(x => x.Title);

        var moduleGroup = new ModuleGroup(header, list);
        Modules.Add(moduleGroup);
    }

    public event DialogWindowResult DialogResultRequested;

    protected virtual void OnDialogCloseRequested(bool dialogresult)
    {
        DialogResultRequested?.Invoke(dialogresult);
    }
}