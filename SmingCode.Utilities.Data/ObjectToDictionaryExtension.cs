using System.ComponentModel;

namespace SmingCode.Utilities.Data;

internal static class ObjectToDictionaryExtension
{
    internal static Dictionary<string, object?> ToDictionary(
        this object anonymousObject
    )
    {
        var result = new Dictionary<string, object?>();

        var objectProperties = TypeDescriptor.GetProperties(anonymousObject);
        foreach (PropertyDescriptor objectProperty in objectProperties)
        {
            result.Add(objectProperty.Name, objectProperty.GetValue(anonymousObject));
        }

        return result;
    }
}