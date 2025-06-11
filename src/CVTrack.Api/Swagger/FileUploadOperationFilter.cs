using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http;

namespace CVTrack.Api.Swagger;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // IFormFile parametresi var mı bakalım
        var hasFileParam = context.MethodInfo
            .GetParameters()
            .Any(p => p.ParameterType == typeof(IFormFile));

        if (!hasFileParam)
            return;

        // multipart/form-data body’sini elle tanımla
        operation.RequestBody = new OpenApiRequestBody
        {
            Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                ["file"] = new OpenApiSchema
                                {
                                    Description = "Yüklenecek PDF dosyası",
                                    Type        = "string",
                                    Format      = "binary"
                                }
                            },
                            Required = new System.Collections.Generic.HashSet<string> { "file" }
                        }
                    }
                }
        };
    }
}