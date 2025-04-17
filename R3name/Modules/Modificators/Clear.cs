using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Modificator("Clear name", "Clears the name.")]
class Clear : Modificator
{
    public override string ProcessFile(ModificatorContext context)
    {
        return string.Empty;
    }
}