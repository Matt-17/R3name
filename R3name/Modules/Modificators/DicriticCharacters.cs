using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Convert diacritic characters", "Converts language specific characters to a more common form (ae -> ä).")]
class DicriticCharacters : Modificator
{
    [Display(Name = "Convert ae -> ä")]
    public bool ConvertForward { get; set; }

    public override string ProcessFile(ModificatorContext context)
    {
        var dict = new Dictionary<string, string>
        {
            { "ae", "ä" },
            { "oe", "ö" },
            { "ue", "ü" },
            { "Ae", "Ä" },
            { "Oe", "Ö" },
            { "Ue", "Ü" },
            { "ss", "ß" }
        };

        foreach (var pair in dict)
        {
            if (ConvertForward)
            {
                context.Filename = context.Filename.Replace(pair.Value, pair.Key);
            }
            else
            {
                context.Filename = context.Filename.Replace(pair.Key, pair.Value);
            }
        }

        return context.Filename;
    }
}