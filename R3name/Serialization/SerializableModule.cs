using System.Collections.Generic;

namespace R3name.Serialization;

internal struct SerializableModule
{
    public string Type { get; set; }
    public bool? Deactivated { get; set; }
    public Dictionary<string, object> Settings { get; set; }
}