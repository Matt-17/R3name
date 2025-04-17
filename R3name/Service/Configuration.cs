using System.Collections.Generic;
using R3name.Modules;
using R3name.Modules.FileSources;
using R3name.Modules.Modificators;

namespace R3name.Service;

internal class Configuration
{
    public string Name { get; set; }

    public FileSource Source { get; set; }
        
    public List<Processor> Processors { get; set; }

    public List<Modificator> Modificators { get; set; }
}