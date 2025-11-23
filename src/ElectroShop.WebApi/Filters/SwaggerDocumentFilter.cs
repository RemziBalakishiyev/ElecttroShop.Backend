using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;

namespace ElectroShop.WebApi.Filters;

/// <summary>
/// IFormFile və Stream type-larını Swagger document-dən tamamilə çıxarır
/// </summary>
public class SwaggerDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // IFormFile və Stream type-larını schema-dan sil
        if (swaggerDoc.Components?.Schemas != null)
        {
            var schemasToRemove = swaggerDoc.Components.Schemas
                .Where(s => 
                    s.Key.Contains("IFormFile", StringComparison.OrdinalIgnoreCase) || 
                    s.Key.Contains("Stream", StringComparison.OrdinalIgnoreCase) ||
                    s.Key.Contains("FormFile", StringComparison.OrdinalIgnoreCase) ||
                    (s.Value.Type == "string" && s.Value.Format == "binary" && 
                     !s.Value.Description?.Contains("File") == true))
                .Select(s => s.Key)
                .ToList();

            foreach (var schemaKey in schemasToRemove)
            {
                swaggerDoc.Components.Schemas.Remove(schemaKey);
            }
        }
    }
}

