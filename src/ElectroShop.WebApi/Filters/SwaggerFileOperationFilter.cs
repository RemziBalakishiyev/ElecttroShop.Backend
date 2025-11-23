using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ElectroShop.WebApi.Filters;

/// <summary>
/// IFormFile parametrlərini Swagger-da düzgün göstərmək üçün filter
/// FromRoute və FromForm parametrlərini düzgün emal edir
/// </summary>
public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var allParameters = context.MethodInfo.GetParameters().ToList();
        
        // IFormFile parametrlərini tap
        var fileParameters = allParameters
            .Where(p => p.ParameterType == typeof(IFormFile) || 
                       p.ParameterType == typeof(IFormFileCollection) ||
                       (p.ParameterType.IsGenericType && 
                        p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                        p.ParameterType.GetGenericArguments()[0] == typeof(IFormFile)))
            .ToList();

        // FromForm parametrlərini tap (IFormFile və digər form parametrləri)
        var formParameters = allParameters
            .Where(p => p.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.FromFormAttribute), false).Any())
            .ToList();

        // FromRoute parametrlərini tap (bunları saxlamalıyıq)
        var routeParameters = allParameters
            .Where(p => p.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.FromRouteAttribute), false).Any())
            .ToList();

        // Əgər IFormFile və ya form parametrləri varsa
        if (fileParameters.Any() || formParameters.Any())
        {
            // Mövcud Parameters-dən yalnız FromForm və IFormFile parametrlərini sil
            // FromRoute parametrlərini saxla
            if (operation.Parameters != null)
            {
                var parametersToRemove = new List<OpenApiParameter>();
                foreach (var param in operation.Parameters)
                {
                    // Yalnız FromForm və ya IFormFile olan parametrləri sil
                    // FromRoute parametrlərini saxla
                    var isFormParam = formParameters.Any(fp => 
                        fp.Name?.Equals(param.Name, StringComparison.OrdinalIgnoreCase) == true);
                    var isFileParam = fileParameters.Any(fp => 
                        fp.Name?.Equals(param.Name, StringComparison.OrdinalIgnoreCase) == true);
                    var isRouteParam = routeParameters.Any(rp => 
                        rp.Name?.Equals(param.Name, StringComparison.OrdinalIgnoreCase) == true);
                    
                    // FromRoute deyilsə və (FromForm və ya IFormFile)-dırsa, sil
                    if (!isRouteParam && (isFormParam || isFileParam))
                    {
                        parametersToRemove.Add(param);
                    }
                }
                
                foreach (var paramToRemove in parametersToRemove)
                {
                    operation.Parameters.Remove(paramToRemove);
                }
            }

            // RequestBody yarad və ya mövcud olanı təmizlə
            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>(),
                            Required = new HashSet<string>()
                        }
                    }
                }
            };

            // IFormFile parametrlərini əlavə et
            foreach (var param in fileParameters)
            {
                var paramName = param.Name ?? "file";
                var schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary",
                    Description = "File to upload"
                };
                operation.RequestBody.Content["multipart/form-data"].Schema.Properties[paramName] = schema;
            }

            // Digər form parametrlərini əlavə et (IFormFile olmayan)
            foreach (var param in formParameters.Where(p => 
                p.ParameterType != typeof(IFormFile) && 
                p.ParameterType != typeof(IFormFileCollection) &&
                !(p.ParameterType.IsGenericType && 
                  p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                  p.ParameterType.GetGenericArguments()[0] == typeof(IFormFile))))
            {
                var paramName = param.Name ?? "value";
                var schema = new OpenApiSchema
                {
                    Type = GetSwaggerType(param.ParameterType),
                    Description = $"Form parameter: {paramName}"
                };
                operation.RequestBody.Content["multipart/form-data"].Schema.Properties[paramName] = schema;
            }
        }
    }

    private static string GetSwaggerType(Type type)
    {
        if (type == typeof(string) || type == typeof(Guid))
            return "string";
        if (type == typeof(int) || type == typeof(int?))
            return "integer";
        if (type == typeof(long) || type == typeof(long?))
            return "integer";
        if (type == typeof(decimal) || type == typeof(decimal?) || type == typeof(double) || type == typeof(double?))
            return "number";
        if (type == typeof(bool) || type == typeof(bool?))
            return "boolean";
        if (type == typeof(DateTime) || type == typeof(DateTime?))
            return "string";
        
        return "string";
    }
}

