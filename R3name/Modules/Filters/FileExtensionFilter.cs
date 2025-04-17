using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

using R3name.Models;
using R3name.Modules.Attributes;
using R3name.Modules.FileSources;

namespace R3name.Modules.Filters;

[Modificator("File extension filter", "File filtering by file extension.")]
public class FileExtensionFilter : FilterProcessor
{
    [Display(Name = "Allowed extensions")]
    [UseTwoLines]
    public string Extensions { get; set; }

    public override bool Filter(IFileDescription file, ModuleArgs args)
    {
        // if no extensions are specified, then all files are allowed
        if (string.IsNullOrWhiteSpace(Extensions))
            return true;

        var allowedExtensions = Extensions.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim().ToLower())
            .Select(x => x.StartsWith('.') ? x : $".{x}")
            .ToList();
        var extension = Path.GetExtension(file.Filename);

        if (string.IsNullOrEmpty(extension))
            return false;

        return allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }
}