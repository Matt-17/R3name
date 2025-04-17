using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using R3name.Modules.Modificators;

using YamlDotNet.Core.Events;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace R3name.Models;

public class ModificatorConverter : JsonConverter<Modificator>
{
    public override Modificator Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonElement = JsonDocument.ParseValue(ref reader).RootElement;
        if (!jsonElement.TryGetProperty("Type", out var typeProperty))
            throw new JsonException("Der Typ ist nicht spezifiziert.");

        var typeName = typeProperty.GetString();
        if (typeName == null)
            throw new JsonException("Der Typ ist nicht spezifiziert.");

        var qualifiedName = typeName;
        if (!typeName.Contains("."))
        {
            qualifiedName = $"{typeof(Modificator).Namespace}.{typeName}";
        }

        var type = Type.GetType(qualifiedName);
        if (type == null)
            throw new JsonException("Unbekannter Typ.");

        return (Modificator)JsonSerializer.Deserialize(jsonElement.GetRawText(), type, options);
    }

    public override void Write(Utf8JsonWriter writer, Modificator value, JsonSerializerOptions options)
    {
        var type = value.GetType();

        writer.WriteStartObject();
        writer.WriteString("Type", type.Name);

        foreach (var property in type.GetProperties())
        {
            if (!property.CanRead)
                continue;

            var propertyValue = property.GetValue(value);
            writer.WritePropertyName(property.Name);
            JsonSerializer.Serialize(writer, propertyValue, propertyValue?.GetType() ?? typeof(object), options);
        }

        writer.WriteEndObject();
    }
}
public class ModificatorYamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return typeof(Modificator).IsAssignableFrom(type);
    }

    public object ReadYaml(IParser parser, Type type)
    {
        var deserializer = new DeserializerBuilder().Build();
        parser.MoveNext(); // Move to the start of the map.

        Modificator result = null;

        while (parser.Current is not MappingEnd)
        {
            var propertyName = parser.Consume<Scalar>().Value;
            if (propertyName == "Type")
            {
                var typeName = parser.Consume<Scalar>().Value;
                var qualifiedName = typeName.Contains(".") ? typeName : $"{typeof(Modificator).Namespace}.{typeName}";
                type = Type.GetType(qualifiedName);
                if (type == null) throw new YamlException("Unbekannter Typ.");
            }
            else
            {
                if (result == null)
                {
                    result = (Modificator)Activator.CreateInstance(type);
                }
                var propertyValue = deserializer.Deserialize(parser, type.GetProperty(propertyName).PropertyType);
                type.GetProperty(propertyName).SetValue(result, propertyValue);
            }
            parser.MoveNext();
        }

        return result;
    }

    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
        var serializer = new SerializerBuilder().Build();

        // Starten der Map für das gesamte Objekt
        emitter.Emit(new MappingStart());

        // Fügen Sie das Typ-Property manuell hinzu
        emitter.Emit(new Scalar(null, "Type"));
        emitter.Emit(new Scalar(null, value.GetType().Name));

        // Verwenden Sie den Serializer für den Rest des Objekts, außer der "Type"-Property
        // Erstellen einer temporären Map ohne die "Type"-Property
        var tempMap = new Dictionary<string, object>();
        foreach (var property in type.GetProperties().Where(p => p.CanRead && p.Name != "Type"))
        {
            var propertyValue = property.GetValue(value);
            tempMap.Add(property.Name, propertyValue);
        }

        // Serialisieren der temporären Map
        serializer.Serialize(emitter, tempMap);

        // Beenden der Map für das gesamte Objekt
        emitter.Emit(new MappingEnd());
    }

}