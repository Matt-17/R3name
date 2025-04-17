using System;

using R3name.Modules.Attributes;

namespace R3name.Modules.Modificators;

[Serializable]
public abstract class Modificator
{
    [Ignore]
    public bool IsDeactivated { get; set; }

    public virtual void Initialize()
    {
        // Initialization is done once on adding this module
    }

    public abstract string ProcessFile(ModificatorContext context);
    //public abstract bool Process(FileDescription fileame);   
}