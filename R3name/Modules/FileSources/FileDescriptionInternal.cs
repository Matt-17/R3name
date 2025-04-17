using System;
using System.IO;

namespace R3name.Modules.FileSources;

public sealed class FileDescriptionInternal : IFileDescription
{
    public FileDescriptionInternal(string path, long size)
    {
        Filename = path;

        FilenameWithoutExtension = Path.GetFileNameWithoutExtension(path);

        Folder = Path.GetDirectoryName(path);
        FilenameOriginal = Path.GetFileName(path);
        Extension = Path.GetExtension(path);
        Size = size;
    }

    /// <summary>
    /// The complete original path of the file. 
    /// </summary>
    public string Filename { get; }

    public string Fullpath { get; }
    public long Size { get; }
    public DateTime? ModifiedDate { get; set; }
    public DateTime? CreatedDate { get; set; }


    /// <summary>
    /// The filename which must be modificated.
    /// </summary>
    public string FilenameWithoutExtension { get; set; }

    public string Text { get; }

    /// <summary>
    /// The original folder
    /// </summary>
    public string Folder { get; }

    /// <summary>
    /// The original filename including the extension.
    /// </summary>
    public string FilenameOriginal { get; }

    /// <summary>
    /// The file extension
    /// </summary>
    public string Extension { get; }

    public bool IsFiltered { get; set; }
    // GROUP
    // FileSource.GetFileStream

}