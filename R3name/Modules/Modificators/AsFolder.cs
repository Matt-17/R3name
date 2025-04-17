using System.IO;
using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("As folder", "Rename like folder")]
public class AsFolder : Modificator
{
    public override string ProcessFile(ModificatorContext context) => new DirectoryInfo(context.Folder).Name;
}