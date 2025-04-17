using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Replace Invalid Chars", "Replaces invalid filename characters with a custom replacement.")]
public class ReplaceInvalidChars : Modificator
{
    private static readonly string InvalidChars = new(Path.GetInvalidFileNameChars());

    [Display(Name = "Replacement", Description = "Character or string to replace invalid chars.")]
    public string Replacement { get; set; } = "_";

    public override string ProcessFile(ModificatorContext context)
    {
        if (string.IsNullOrEmpty(context.Filename))
        {
            return context.Filename;
        }

        var fileName = new StringBuilder(context.Filename.Length);

        foreach (var c in context.Filename)
        {
            fileName.Append(InvalidChars.Contains(c) ? Replacement : c.ToString());
        }

        return fileName.ToString();
    }
}
