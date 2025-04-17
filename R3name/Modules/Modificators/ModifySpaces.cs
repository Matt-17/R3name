using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Modify spaces", "Kills double spaces and also spaces at the beginning or the end of the word.")]
class ModifySpaces : Modificator
{
    [Display(Name = "Convert underscore to space")]
    public bool ConvertUnderscoreToSpace { get; set; }

    [Display(Name = "Convert dot to space")]
    public bool ConvertDotToSpace { get; set; }

    [Display(Name = "Kill double spaces")]
    public bool KillDoubleSpaces { get; set; }

    [Display(Name = "Kill spaces at start")]
    public bool KillStartSpaces { get; set; }

    [Display(Name = "Kill spaces at end")]
    public bool KillEndSpaces { get; set; }


    public override string ProcessFile(ModificatorContext context)
    {
        if (ConvertUnderscoreToSpace)
            context.Filename = context.Filename.Replace("_", " ");
        if (ConvertDotToSpace)
            context.Filename = context.Filename.Replace(".", " ");
        if (KillDoubleSpaces)
            context.Filename = Regex.Replace(context.Filename, "([ ]+)", " ");
        if (KillStartSpaces)
            context.Filename = Regex.Replace(context.Filename, "^([ ]+)", "");
        if (KillEndSpaces)
            context.Filename = Regex.Replace(context.Filename, "([ ]+)$", "");
        return context.Filename;
    }
}