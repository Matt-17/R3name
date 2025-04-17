using System.Collections.Generic;

namespace R3name.Serialization;

internal struct SerializationContext
{
    public string Name { get; set; }
    public List<SerializableModule> Modificators { get; set; }
    public List<SerializableModule> Processors { get; set; }
}