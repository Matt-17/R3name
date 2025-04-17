using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Regex replace", "Replaces any text by a regex search pattern and uses regex replacement.")]
public class RegexReplace : Modificator
{
    [Display(Name = "Regex pattern")]
    [UseTwoLines]
    public string Pattern { get; set; }


    [Display(Name = "Ignore case")]
    public bool IgnoreCase { get; set; }

    [UseTwoLines]
    public string Replacement { get; set; }

    public override string ProcessFile(ModificatorContext context)
    {
        if (string.IsNullOrEmpty(Pattern))
            return context.Filename;

        var replacement = Replacement ?? string.Empty;

        var options = RegexOptions.None;
        if (IgnoreCase)
            options |= RegexOptions.IgnoreCase;

        try
        {
            var regex = new Regex(Pattern, options);
            return regex.Replace(context.Filename, replacement);
        }
        catch (System.Exception)
        {
            return context.Filename;
        }
    }
}