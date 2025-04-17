using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using R3name.Models;
using R3name.Modules.FileSources;

namespace R3name.Modules;

public abstract class Processor
{
    public virtual void Initialize()
    {
        // Initialization is done once on adding this module
    }

    protected virtual void OnBeforeProcess()
    {
        // 
    }

    public IEnumerable<IFileDescription> Process(IEnumerable<IFileDescription> files, ModuleArgs args)
    {
        Debug.Assert(files != null, "files must not be null");
        Debug.Assert(args != null, "args must not be null");

        var fileList = files.ToList();

        OnBeforeProcess();
        try
        {
            files = OnProcess(fileList, args);
        }
        catch (System.Exception ex)
        {
            args.ErrorMessage = $"This module threw an exception ({ex.Message})";
            return fileList;
        }

        if (args.ErrorMessage != null)
            return fileList;


        if (files == null)
        {
            args.ErrorMessage = "This module returned null";
            return fileList;
        }

        OnAfterProcess();
        return files;
    }

    protected virtual void OnAfterProcess()
    {

    }

    protected abstract IEnumerable<IFileDescription> OnProcess(IEnumerable<IFileDescription> files, ModuleArgs args);
}