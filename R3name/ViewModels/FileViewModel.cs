using System.IO;

using R3name.Modules.FileSources;

namespace R3name.ViewModels;

public class FileViewModel : BaseViewModel
{
    private string _text;
    private readonly string _path;
    private string _filenameNew;

    public FileViewModel(string path)
    {
        _path = path;
        FilenameNew = FilenameOld;
    }

    public FileViewModel(FileDescriptionInternal fileDescription)
    {
        _path = fileDescription.Filename;
        FilenameNew = fileDescription.FilenameWithoutExtension;
        IsFiltered = fileDescription.IsFiltered;
    }

    public string PathOriginal => _path;
    public string Text
    {
        get => _text;
        set
        {
            if (value == _text) return;
            _text = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FilenameNew));
        }
    }

    public ChangeStatus Status { get; set; }

    public bool IsFiltered { get; set; }

    public string FilenameOld => Path.GetFileName(_path);

    public string FilenameNew
    {
        get => _filenameNew;
        set
        {
            if (value == _filenameNew) return;
            _filenameNew = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsChanged));
        }
    }

    public string Folder => Path.GetDirectoryName(_path);
    public bool IsChanged => !string.Equals(FilenameOld, FilenameNew);

    public string GetRequestedPath() => Path.Combine(Folder, FilenameNew);
}

public class ChangeStatus
{
    public ChangeStatusTypes ChangeStatusType { get; }
    public bool IsSuccess => ChangeStatusType is ChangeStatusTypes.Unchanged or ChangeStatusTypes.FilenameChanged or ChangeStatusTypes.FilenameChangedCaseOnly;
    public string ConflictingFilename { get; }
    public char? InvalidCharacter { get; }
    public string Message { get; }

    private ChangeStatus(ChangeStatusTypes changeStatusType, string message, string conflictingFilename = null, char? invalidCharacter = null)
    {
        ChangeStatusType = changeStatusType;
        Message = message;
        ConflictingFilename = conflictingFilename;
        InvalidCharacter = invalidCharacter;
    }

    public static ChangeStatus Unchanged() => new(ChangeStatusTypes.Unchanged, "The filename is unchanged.");
    public static ChangeStatus FilenameChanged() => new(ChangeStatusTypes.FilenameChanged, "The filename was changed.");
    public static ChangeStatus DuplicateError(string existingFilename) => new(ChangeStatusTypes.DuplicateError, $"Filename conflicts with existing file: {existingFilename}", existingFilename);
    public static ChangeStatus InvalidCharsError(char invalidChar) => new(ChangeStatusTypes.InvalidCharsError, $"Filename contains an invalid character: '{invalidChar}'", invalidCharacter: invalidChar);
    public static ChangeStatus FilenameChangedCaseOnly() => new(ChangeStatusTypes.FilenameChangedCaseOnly, "Only the case of the filename was changed.");
    public static ChangeStatus TooLongError(int maxLength) => new(ChangeStatusTypes.TooLongError, $"The filename exceeds the maximum length of {maxLength} characters.");
    public static ChangeStatus EmptyFilenameError() => new(ChangeStatusTypes.EmptyFilenameError, "The filename cannot be empty.");
    public static ChangeStatus ReservedNameError(string reservedName) => new(ChangeStatusTypes.ReservedNameError, $"The filename '{reservedName}' is reserved and cannot be used.", reservedName);
    public static ChangeStatus PathTooLongError(int maxLength) => new(ChangeStatusTypes.PathTooLongError, $"The full path exceeds the allowed length of {maxLength} characters.");
    public static ChangeStatus UnauthorizedError() => new(ChangeStatusTypes.UnauthorizedError, "You do not have permission to rename this file.");
    public static ChangeStatus IoError(string details) => new(ChangeStatusTypes.IoError, $"An I/O error occurred while renaming the file: {details}.");
    public static ChangeStatus UnknownError() => new(ChangeStatusTypes.UnknownError, "An unknown error occurred.");

    public override string ToString() => Message;
    public enum ChangeStatusTypes
    {
        Unchanged,               // Der Dateiname wurde nicht verändert
        FilenameChanged,         // Der Dateiname wurde geändert
        FilenameChangedCaseOnly, // Nur die Groß-/Kleinschreibung wurde geändert
        DuplicateError,          // Der neue Name existiert bereits
        InvalidCharsError,       // Ungültige Zeichen im Dateinamen
        TooLongError,            // Der Dateiname überschreitet die maximale Länge
        EmptyFilenameError,      // Der Dateiname ist leer
        ReservedNameError,       // Der Dateiname ist ein reservierter Systemname (z. B. CON, NUL, AUX unter Windows)
        PathTooLongError,        // Der komplette Pfad ist zu lang
        UnauthorizedError,       // Keine Berechtigung zum Umbenennen
        IoError,                 // Allgemeiner Ein-/Ausgabe-Fehler beim Umbenennen
        UnknownError             // Ein unbekannter Fehler ist aufgetreten
    }
}
