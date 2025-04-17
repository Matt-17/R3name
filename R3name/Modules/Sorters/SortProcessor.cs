using System.Collections.Generic;
using R3name.Models;
using R3name.Modules.FileSources;

namespace R3name.Modules.Sorters;

public abstract class SortProcessor : Processor
{
    protected override IEnumerable<IFileDescription> OnProcess(IEnumerable<IFileDescription> files, ModuleArgs args)
    {
        // Sort processor does nothing in specific

        return Sort(files, args);
    }

    public abstract IEnumerable<IFileDescription> Sort(IEnumerable<IFileDescription> fileDescriptions, ModuleArgs args);
}