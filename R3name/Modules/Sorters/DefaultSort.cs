using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using R3name.Models;
using R3name.Modules.Attributes;
using R3name.Modules.FileSources;

namespace R3name.Modules.Sorters;

[Modificator("File sorting", "File sorting by Filename")]
class DefaultSort : SortProcessor
{
    public enum SortTypes
    {
        [Display(Name = "Don't sort")]
        None,
        [Display(Name = "By filename")]
        Filename,



        [Display(Name = "By text value")]
        Text = 99,
    }

    [Display(Name = "Sorting property")]
    [UseTwoLines]
    public SortTypes SortType { get; set; }

    [Display(Name = "Descending order")]
    public bool Descending { get; set; }
         
    public override IEnumerable<IFileDescription> Sort(IEnumerable<IFileDescription> files, ModuleArgs args)
    {
        switch (SortType)
        {
            case SortTypes.None:
                break;
            case SortTypes.Filename:
                files = files.OrderBy(x => x.FilenameWithoutExtension);
                break;
            case SortTypes.Text:
                files = files.OrderBy(x => x.Text);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (Descending)
            files = files.Reverse();

        return files;
    }
}