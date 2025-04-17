using System;
using System.ComponentModel.DataAnnotations;

using R3name.Models;
using R3name.Modules.Attributes;
using R3name.Modules.FileSources;

namespace R3name.Modules.Filters;

[Modificator("File size filter", "File filtering by file size.")]
public class FileSizeFilter : FilterProcessor
{
    [Display(Name = "Minimum file size (bytes)")]
    [Numeric(Minimum = 0)]
    [UseTwoLines]
    public int MinSize { get; set; }

    [Display(Name = "Maximum file size (bytes)")]
    [Numeric(Minimum = 0)]
    [UseTwoLines]
    public int MaxSize { get; set; }

    public override bool Filter(IFileDescription file, ModuleArgs args)
    {
        var size = file.Size;

        // console log file size, min and max with Console.WriteLine
        Console.WriteLine($"File size: {size}, min: {MinSize}, max: {MaxSize}");

        // ReSharper disable once ReplaceWithSingleAssignment.True
        var result = true;

        if (MinSize != 0 && size < MinSize)
            result = false;
        if (MaxSize != 0 && size > MaxSize)
            result = false;

        return result;
    }

}