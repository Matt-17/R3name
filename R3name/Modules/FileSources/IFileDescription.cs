using System;

namespace R3name.Modules.FileSources;

public interface IFileDescription
{
    string FilenameWithoutExtension { get; }
    string Text { get; }
    string Filename { get; }
    string Extension { get; }
    string Fullpath { get; }
        
    long Size { get; }
    DateTime? ModifiedDate { get; }
    DateTime? CreatedDate { get; }
}