using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

using Microsoft.WindowsAPICodePack.Dialogs;

using R3name.Helper;
using R3name.Modules.Attributes;
using R3name.Properties;

namespace R3name.Modules.FileSources;

[Modificator("File selector", "Default processor for retrieving files")]
public class LocalFolderSource : FileSource
{
    private bool _includeSubdirectories;
    private int _subdirectoryDepthLimit;
    private bool _includeHiddenFiles;
    private bool _includeSystemFiles;
    private string _searchPattern;

    private LocalFolderWatcher _watcher;
    private string _selectedFolder;

    public string SelectedFolder
    {
        get => _selectedFolder;
        set
        {
            if (value == _selectedFolder) return;
            _selectedFolder = value;
            OnPropertyChanged();
            OnRefreshNeeded();
        }
    }

    public LocalFolderSource()
    {
        LoadSettings();
        ChooseDirectoryCommand = new RelayCommand(ChooseDirectory);
        SearchPattern = "*.*";
    }

    public bool IncludeSubdirectories
    {
        get => _includeSubdirectories;
        set
        {
            if (value == _includeSubdirectories) return;
            _includeSubdirectories = value;
            OnPropertyChanged();
            OnRefreshNeeded();
        }
    }

    public int SubdirectoryDepthLimit
    {
        get => _subdirectoryDepthLimit;
        set
        {
            if (value == _subdirectoryDepthLimit) return;
            _subdirectoryDepthLimit = value;
            OnPropertyChanged();
            OnRefreshNeeded();
        }
    }

    public bool DoMatchCasing { get; set; }

    public bool IncludeHiddenFiles
    {
        get => _includeHiddenFiles;
        set
        {
            if (value == _includeHiddenFiles) return;
            _includeHiddenFiles = value;
            OnPropertyChanged();
            OnRefreshNeeded();
        }
    }

    public bool IncludeSystemFiles
    {
        get => _includeSystemFiles;
        set
        {
            if (value == _includeSystemFiles) return;
            _includeSystemFiles = value;
            OnPropertyChanged();
            OnRefreshNeeded();
        }
    }

    public string SearchPattern
    {
        get => _searchPattern;
        set
        {
            if (value == _searchPattern) return;
            _searchPattern = value;
            OnPropertyChanged();
            OnRefreshNeeded();
        }
    }

    public ICommand ChooseDirectoryCommand { get; }

    private void SetPath(string fileName)
    {
        if (_watcher != null)
        {
            _watcher.Changed -= OnRefreshFiles;
            _watcher = null;
        }

        var newFolder = GetExistingParentFolder(fileName);

        if (!Directory.Exists(newFolder))
            newFolder = null;

        if (string.IsNullOrWhiteSpace(newFolder))
            newFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        _watcher = new LocalFolderWatcher(newFolder);
        _watcher.Changed += OnRefreshFiles;

        SelectedFolder = newFolder;
        OnPropertyChanged(nameof(newFolder));
    }

    private void OnRefreshFiles(object sender, FileSystemEventArgs e)
    {
        OnRefreshNeeded();
    }

    private void LoadSettings()
    {
        SetPath(Settings.Default.Folder);
    }

    private void ChooseDirectory()
    {
        var selectedFolder = GetExistingParentFolder(SelectedFolder);

        var dlg = new CommonOpenFileDialog
        {
            Title = "Choose direktory",
            IsFolderPicker = true,
            InitialDirectory = selectedFolder,

            AddToMostRecentlyUsedList = false,
            AllowNonFileSystemItems = false,
            DefaultDirectory = selectedFolder,
            EnsureFileExists = true,
            EnsurePathExists = true,
            EnsureReadOnly = true,
            EnsureValidNames = true,
            Multiselect = false,
            ShowPlacesList = true
        };

        if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
        {
            SetValue(dlg.FileName);
        }
    }

    private static string GetExistingParentFolder(string selectedFolder)
    {
        try
        {
            var dirInfo = new DirectoryInfo(selectedFolder);
            if (dirInfo.Exists)
                return dirInfo.FullName;

            while (!dirInfo.Exists)
            {
                dirInfo = dirInfo.Parent;
                if (dirInfo == null)
                {
                    dirInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    break;
                }
            }

            return dirInfo.FullName;
        }
        catch (Exception)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }

    private void SaveSettings()
    {
        Settings.Default.Folder = SelectedFolder;
        Settings.Default.Save();
    }
    public override IEnumerable<SourceFile> GetFiles()
    {
        var result = new List<SourceFile>();

        var files = GetAllFiles(SelectedFolder, 0);
        foreach (var file in files)
        {
            result.Add(new LocalFolderSourceFile(file));
        }

        return result;
    }

    /// <summary>
    /// Determines whether the given filename is a valid file name.
    /// </summary>
    /// <param name="filename">The filename to check.</param>
    /// <returns>True if the filename is valid, false otherwise.</returns>
    /// <remarks>
    /// A valid file name is a non-empty string that does not contain any of the characters in <see cref="Path.GetInvalidFileNameChars"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var isValid = IsValidFilename("file1.txt"); // true
    /// var isValid = IsValidFilename("file:1.txt"); // false
    /// </code>
    /// </example>
    public override bool IsValidFilename(string filename)
    {
        // empty or null return fallse
        if (string.IsNullOrWhiteSpace(filename))
            return false;

        // use Path.GetInvalidFileNameChars
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            if (filename.Contains(c))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Finds duplicate filenames in the given list.
    /// </summary>
    /// <param name="filenames">The list of filenames to search for duplicates.</param>
    /// <returns>A list of duplicate filenames.</returns>
    /// /// <remarks>
    /// The search is case-insensitive if the <see cref="DoMatchCasing"/> property is set to false.
    /// </remarks>
    /// <example>
    /// <code>
    /// var duplicates = FindDuplicates(new List&gt;string&lt; {"file1.txt", "File1.txt", "file2.txt"});
    /// // duplicates will contain "file1.txt" and "File1.txt"
    /// </code>
    /// </example>
    public override List<string> FindDuplicates(IReadOnlyList<string> filenames)
    {
        var list = new List<string>(filenames);

        if (!DoMatchCasing)
        {
            list = list.Select(x => x.ToLower()).ToList();
        }

        var duplicates = list.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();

        return duplicates;
    }

    public override bool SupportsFileDates => true;

    public override bool AcceptsValue(string data) => Directory.Exists(data);

    public override void SetValue(string data)
    {
        SetPath(data);
        SaveSettings();
        OnRefreshNeeded();
    }

    private IEnumerable<string> GetAllFiles(string path, int subfolderCount)
    {
        var options = new EnumerationOptions();
        if (DoMatchCasing)
            options.MatchCasing = MatchCasing.CaseSensitive;
        options.AttributesToSkip = FileAttributes.Hidden | FileAttributes.System;
        if (IncludeHiddenFiles)
            options.AttributesToSkip ^= FileAttributes.Hidden;
        if (IncludeSystemFiles)
            options.AttributesToSkip ^= FileAttributes.System;

        var files = Directory.EnumerateFiles(path, SearchPattern, options);
        foreach (var file in files)
        {
            yield return file;
        }

        if (!IncludeSubdirectories)
            yield break;

        if (SubdirectoryDepthLimit > 0 && subfolderCount >= SubdirectoryDepthLimit)
            yield break;

        var directories = Directory.EnumerateDirectories(path);

        foreach (var directory in directories)
        {
            foreach (var file in GetAllFiles(directory, subfolderCount + 1))
            {
                yield return file;
            }
        }
    }
}
public class LocalFolderWatcher
{
    private readonly FileSystemWatcher _watcher;

    public event FileSystemEventHandler Changed;

    public LocalFolderWatcher(string pathToWatch)
    {
        _watcher = new FileSystemWatcher();
        _watcher.Path = pathToWatch;

        // Beobachtet sowohl Dateien als auch Unterverzeichnisse
        _watcher.IncludeSubdirectories = true;

        // Beobachtet alle Änderungen in Dateien und Ordnern
        _watcher.NotifyFilter = NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite
                               | NotifyFilters.FileName
                               | NotifyFilters.DirectoryName;

        // Abonnieren von Ereignissen
        _watcher.Changed += OnChanged;
        _watcher.Created += OnChanged;
        _watcher.Deleted += OnChanged;
        _watcher.Renamed += OnRenamed;

        // Überwachung starten
        _watcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        Changed?.Invoke(this, e);
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        Changed?.Invoke(this, e);
    }
}
