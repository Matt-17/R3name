using System.ComponentModel.DataAnnotations;
using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Convert defaults", "Converts accents to apostrophes.")]
class ConvertCharacters : Modificator
{
    [Display(Name = "Convert '´' & '`' ➞ '''")]
    public bool ConvertAccentsToApostrophes { get; set; }

    [Display(Name = "Convert '.' ➞ ' '")]
    public bool ConvertDotsToSpaces { get; set; }

    public override string ProcessFile(ModificatorContext context)
    {
        if (ConvertAccentsToApostrophes)
            context.Filename = context.Filename.Replace('`', '\'').Replace('´', '\'');
        if (ConvertDotsToSpaces)
            context.Filename = context.Filename.Replace('.', ' ');
        return context.Filename;
    }
}