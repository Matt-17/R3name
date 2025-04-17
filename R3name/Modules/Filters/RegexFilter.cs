using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using R3name.Models;
using R3name.Modules.Attributes;
using R3name.Modules.FileSources;

namespace R3name.Modules.Filters;

[Modificator("Regex filter", "File filtering by regex search pattern.")]
public class RegexFilter : FilterProcessor
{
    [Display(Name = "Regex pattern")]
    [UseTwoLines]
    public string Pattern { get; set; }

    [Display(Name = "Ignore case")]
    public bool IgnoreCase { get; set; }

    public override bool Filter(IFileDescription file, ModuleArgs args)
    {
        if (string.IsNullOrEmpty(Pattern))
        {
            args.ErrorMessage = "Pattern must not be empty.";
            return false;
        }

        var options = RegexOptions.None;
        if (IgnoreCase)
            options |= RegexOptions.IgnoreCase;



        Console.WriteLine("RegexFilter: " + Pattern + " " + file.Filename + " " + options);

        var regex = new Regex(Pattern, options);

        return regex.IsMatch(file.Filename);
    }
}