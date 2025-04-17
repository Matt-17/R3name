using R3name.ViewModels;

namespace R3name.Models.Interfaces;

public interface IWindowService
{
    bool ShowSelectModuleDialog(SelectModuleViewModel vm);
}