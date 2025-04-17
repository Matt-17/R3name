using System;
using System.ComponentModel.DataAnnotations;

using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Replace", "Replaces a word by another word.")]
class Replace : Modificator
{
    [Display(Name = "Replace this")]
    [UseTwoLines]
    public string Search { get; set; }

    [Display(Name = "by that")]
    [UseTwoLines]
    public string Replacement { get; set; }

    [Display(Name = "Ignore case")]
    public bool IgnoreCase { get; set; }

    public override string ProcessFile(ModificatorContext context)
    {
        if (string.IsNullOrEmpty(Search))
            return context.Filename;

        return context.Filename.Replace(Search, Replacement, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }
}