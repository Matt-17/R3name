using System.IO;

namespace R3name.Modules.FileSources;

internal class LocalFolderSourceFile : SourceFile
{
    private readonly FileDescriptionInternal _fileDescriptionInternal;

    public LocalFolderSourceFile(string file)
    {
        var fileInfo = new FileInfo(file);
        _fileDescriptionInternal = new FileDescriptionInternal(file, fileInfo.Length)
        {
            ModifiedDate = fileInfo.LastWriteTime,
            CreatedDate = fileInfo.CreationTime
        };
    }

    public override FileDescriptionInternal CreateDescriptor()
    {
        return _fileDescriptionInternal;
    }
}