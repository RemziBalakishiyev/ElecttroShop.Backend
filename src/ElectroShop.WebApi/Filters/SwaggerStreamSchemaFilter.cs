using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ElectroShop.WebApi.Filters;

/// <summary>
/// Stream və IFormFile type-larını Swagger schema-dan gizlədən filter
/// </summary>
public class SwaggerStreamSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Stream type-ları üçün
        if (context.Type == typeof(Stream) || context.Type.IsSubclassOf(typeof(Stream)))
        {
            schema.Type = "string";
            schema.Format = "binary";
            schema.Description = "File stream (multipart/form-data)";
        }

        // IFormFile type-ları üçün
        if (context.Type == typeof(IFormFile) || context.Type == typeof(IFormFileCollection))
        {
            schema.Type = "string";
            schema.Format = "binary";
            schema.Description = "File upload (multipart/form-data)";
        }

        if (schema.Properties != null)
        {
            var propertiesToRemove = new List<string>();
            foreach (var property in schema.Properties)
            {
                var propertyInfo = context.Type.GetProperty(property.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    var propType = propertyInfo.PropertyType;
                    if (propType == typeof(Stream) || 
                        propType.IsSubclassOf(typeof(Stream)) ||
                        propType == typeof(IFormFile) ||
                        propType == typeof(IFormFileCollection))
                    {
                        propertiesToRemove.Add(property.Key);
                    }
                }
            }

            foreach (var propertyKey in propertiesToRemove)
            {
                schema.Properties.Remove(propertyKey);
            }
        }
    }
}



