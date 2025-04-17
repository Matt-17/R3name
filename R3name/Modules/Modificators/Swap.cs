using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Swapping", "Swap parts by seperating")]
public class Swap : Modificator
{
    [Display(Name = "Separator", Description = "The string used to split the filename.")]
    public string Seperator { get; set; } = "-";

    [Display(Name = "Occurrence", Description = "The index of the desired occurrence of the separator.")]
    [Numeric(Minimum = 1)]
    public int Occurrence { get; set; } = 1;

    [Display(Name = "Enable Partial Swap", Description = "Swap only part before occurrence and part after occurrence.")]
    public bool EnablePartialSwap { get; set; }

    [Display(Name = "Limit", Description = "Limit the occurrence to the maximum available.")]
    public bool LimitOccurence { get; set; }

    public override string ProcessFile(ModificatorContext context)
    {
        if (string.IsNullOrEmpty(Seperator) || !context.Filename.Contains(Seperator))
            return context.Filename;

        var parts = context.Filename.Split(new[] { Seperator }, StringSplitOptions.None);

        if (parts.Length < 2)
            return context.Filename;

        var occurrence = Math.Max(1, Occurrence);
        if (occurrence > parts.Length - 1)
        {
            if (!LimitOccurence)
                return context.Filename;

            occurrence = parts.Length - 1;
        }

        if (EnablePartialSwap)
        {
            // swap only part before occurrence and part after occurrence
            (parts[occurrence - 1], parts[occurrence]) = (parts[occurrence], parts[occurrence - 1]);
        }
        else
        {
            // swap the whole parts
            parts = parts[occurrence..].Concat(parts[..occurrence]).ToArray();
        }

        return string.Join(Seperator, parts);
    }
}