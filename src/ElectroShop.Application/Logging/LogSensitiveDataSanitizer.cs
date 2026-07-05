using System.Text.Json;
using System.Text.Json.Nodes;

namespace ElectroShop.Application.Logging;

public static class LogSensitiveDataSanitizer
{
    private const int MaxSerializedLength = 8000;

    private static readonly HashSet<string> SensitivePropertyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "password",
        "passwordhash",
        "newpassword",
        "oldpassword",
        "confirmpassword",
        "token",
        "accesstoken",
        "refreshtoken",
        "authorization",
        "otp",
        "otpcode",
        "secret",
        "signingkey",
        "apikey",
        "creditcard",
        "cardnumber",
        "cvv",
        "pin"
    };

    public static string? SanitizeJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;

        try
        {
            var node = JsonNode.Parse(json);
            if (node is null)
                return json;

            RedactNode(node);
            var sanitized = node.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
            return Truncate(sanitized);
        }
        catch (JsonException)
        {
            return Truncate(json);
        }
    }

    public static string? SanitizeObject(object? value)
    {
        if (value is null)
            return null;

        try
        {
            var json = JsonSerializer.Serialize(value);
            return SanitizeJson(json);
        }
        catch
        {
            return "[Unserializable payload]";
        }
    }

    public static string Truncate(string? value, int maxLength = MaxSerializedLength)
    {
        if (string.IsNullOrEmpty(value))
            return value ?? string.Empty;

        return value.Length <= maxLength
            ? value
            : value[..maxLength] + "...[truncated]";
    }

    private static void RedactNode(JsonNode node)
    {
        switch (node)
        {
            case JsonObject obj:
                foreach (var property in obj.ToList())
                {
                    if (property.Key is not null && SensitivePropertyNames.Contains(property.Key))
                    {
                        obj[property.Key] = "***REDACTED***";
                        continue;
                    }

                    if (property.Value is not null)
                        RedactNode(property.Value);
                }
                break;

            case JsonArray array:
                foreach (var item in array)
                {
                    if (item is not null)
                        RedactNode(item);
                }
                break;
        }
    }
}
