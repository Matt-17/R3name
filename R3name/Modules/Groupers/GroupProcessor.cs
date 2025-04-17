using System.Collections.Generic;
using System.Linq;
using R3name.Models;
using R3name.Modules.FileSources;

namespace R3name.Modules.Groupers;

public abstract class GroupProcessor : Processor
{
    protected override IEnumerable<IFileDescription> OnProcess(IEnumerable<IFileDescription> files, ModuleArgs args)
    {
        var fileDescriptions = files.ToList();
        foreach (var file in fileDescriptions)
        {
            var groupString = Group(file, args);

            if (args.ErrorMessage != null)
                return null;

            // TODO add group to fileDescription
        }
        return fileDescriptions;
    }
    public abstract string Group(IFileDescription file, ModuleArgs args);
}