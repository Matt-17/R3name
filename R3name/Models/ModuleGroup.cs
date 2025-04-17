using System.Collections.Generic;

namespace R3name.Models;

public class ModuleGroup : List<ModuleDescription>
{
    public ModuleGroup(string header, IEnumerable<ModuleDescription> descriptions) : base(descriptions)
    {
        Header = header;

    }

    public string Header { get; set; }
}