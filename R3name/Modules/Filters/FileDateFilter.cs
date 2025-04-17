using System;
using System.ComponentModel.DataAnnotations;
using R3name.Models;
using R3name.Modules.Attributes;
using R3name.Modules.FileSources;

namespace R3name.Modules.Filters;

[Modificator("File date filter", "File filtering by modified date.")]
public class FileDateFilter : FilterProcessor
{
    [Display(Name = "Minimum modified date")]
    [UseTwoLines]
    public DateTime MinDate { get; set; }

    [Display(Name = "Maximum modified date")]
    [UseTwoLines]
    public DateTime MaxDate { get; set; }

    public override bool Filter(IFileDescription file, ModuleArgs args)
    {
        var modifiedDate = file.ModifiedDate;
        return modifiedDate >= MinDate && modifiedDate <= MaxDate;
    }
}