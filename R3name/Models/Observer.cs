using R3name.Models.Interfaces;

namespace R3name.Models;

public class Observer
{
    private IModuleObserver _observer;
    private static Observer _instance;

    public static Observer Manager
    {
        get
        {
            if (_instance == null)
                _instance = new Observer();
            return _instance;
        }
    }

    public void Subscribe(IModuleObserver observer)
    {
        _observer = observer;
    }

    public void NotifyModificatorChange()
    {
        if (_observer == null)
            return;
        _observer.Refresh();
    }
}