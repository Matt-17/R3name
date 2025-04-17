using System.Collections.Generic;
using System.Linq;
using R3name.Models;
using R3name.Modules.FileSources;

namespace R3name.Modules.Filters;

public abstract class FilterProcessor : Processor
{
    protected override IEnumerable<IFileDescription> OnProcess(IEnumerable<IFileDescription> files, ModuleArgs args)
    {
        var fileList = files.ToList();

        foreach (var file in fileList.Cast<FileDescriptionInternal>())
        {
            if (file.IsFiltered)
                continue;

            file.IsFiltered = !Filter(file, args);
        }

        return fileList;
    }
    public abstract bool Filter(IFileDescription file, ModuleArgs args);
}