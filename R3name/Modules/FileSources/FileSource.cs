using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace R3name.Modules.FileSources;

public abstract class FileSource : INotifyPropertyChanged
{
    public abstract IEnumerable<SourceFile> GetFiles();
    public abstract bool IsValidFilename(string filename);
    public abstract List<string> FindDuplicates(IReadOnlyList<string> filenames);
    public abstract bool SupportsFileDates { get; }

    // event RefreshNeeded
    internal event EventHandler<RefreshNeededEventHandlerArgs> Changed;


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected virtual void OnRefreshNeeded()
    {
        var e = new RefreshNeededEventHandlerArgs();

        Changed?.Invoke(this, e);
    }

    public abstract bool AcceptsValue(string data);

    public abstract void SetValue(string data);
}