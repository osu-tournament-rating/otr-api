using System.Reflection;
using Newtonsoft.Json;

namespace API.Utilities;

/// <summary>
/// Custom Newtonsoft.Json converter that ensures enum dictionary keys are serialized as numeric values
/// instead of description strings.
/// </summary>
public class NewtonsoftEnumDictionaryKeyConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        // This converter handles dictionaries where the key is an enum
        if (!objectType.IsGenericType)
        {
            return false;
        }

        Type genericDefinition = objectType.GetGenericTypeDefinition();
        if (genericDefinition != typeof(Dictionary<,>) &&
            genericDefinition != typeof(IDictionary<,>) &&
            !objectType.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
        {
            return false;
        }

        Type keyType = objectType.GetGenericArguments()[0];
        return keyType.IsEnum;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }

        Type keyType = objectType.GetGenericArguments()[0];
        Type valueType = objectType.GetGenericArguments()[1];

        // Create the dictionary instance
        Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        object dictionary = Activator.CreateInstance(dictionaryType)!;
        MethodInfo addMethod = dictionaryType.GetMethod("Add")!;

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                throw new JsonException("Expected PropertyName token");
            }

            string keyString = reader.Value?.ToString() ?? throw new JsonException("Property name cannot be null");

            // Try to parse as integer first, then fall back to string parsing
            object key;
            if (int.TryParse(keyString, out int intValue) && Enum.IsDefined(keyType, intValue))
            {
                key = Enum.ToObject(keyType, intValue);
            }
            else if (Enum.TryParse(keyType, keyString, true, out object? enumValue))
            {
                key = enumValue;
            }
            else
            {
                throw new JsonException($"Unable to convert '{keyString}' to {keyType.Name}");
            }

            reader.Read();
            object? value = serializer.Deserialize(reader, valueType);
            addMethod.Invoke(dictionary, [key, value]);
        }

        return dictionary;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();

        Type dictionaryType = value.GetType();

        // Get the dictionary entries
        PropertyInfo keysProperty = dictionaryType.GetProperty("Keys")!;
        PropertyInfo indexer = dictionaryType.GetProperty("Item")!;
        System.Collections.IEnumerable keys = (System.Collections.IEnumerable)keysProperty.GetValue(value)!;

        foreach (object? key in keys)
        {
            // Write enum key as its numeric value converted to string
            string numericKey = Convert.ToInt32(key).ToString();
            writer.WritePropertyName(numericKey);

            object? val = indexer.GetValue(value, [key]);
            serializer.Serialize(writer, val);
        }

        writer.WriteEndObject();
    }
}
