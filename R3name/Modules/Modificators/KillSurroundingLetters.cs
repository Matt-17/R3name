using System.ComponentModel.DataAnnotations;
using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Kill surrounding letters", "Kills a certain number of letters at the beginning and at the end.")]
class KillSurroundingLetters : Modificator
{
    [Display(Name = "Remove leading")]
    [Numeric(Minimum = 0)]
    public int LeadingCharCount { get; set; }

    [Display(Name = "Remove ending")]
    [Numeric(Minimum = 0)]
    public int TrailingCharCount { get; set; }

    public override string ProcessFile(ModificatorContext context)
    {
        var countStart = LeadingCharCount;
        if (countStart > context.Filename.Length)
            countStart = context.Filename.Length;

        var length = context.Filename.Length - LeadingCharCount - TrailingCharCount;
        if (length < 0)
            length = 0;

        context.Filename = context.Filename.Substring(countStart, length);
        return context.Filename;
    }
}