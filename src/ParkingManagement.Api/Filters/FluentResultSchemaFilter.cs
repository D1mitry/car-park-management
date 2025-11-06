using FluentResults;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ParkingManagement.Api.Filters;

internal sealed class FluentResultSchemaFilter : ISchemaFilter, IDocumentFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType = type.GetGenericArguments()[0];
            var innerSchema = context.SchemaGenerator.GenerateSchema(innerType, context.SchemaRepository);

            // Replace the Result<T> schema with the inner type's schema
            schema.Type = innerSchema.Type;
            schema.Properties = innerSchema.Properties;
            schema.Required = innerSchema.Required;
            schema.AdditionalProperties = innerSchema.AdditionalProperties;
            schema.Reference = innerSchema.Reference;
            schema.AllOf = innerSchema.AllOf;
            schema.OneOf = innerSchema.OneOf;
            schema.AnyOf = innerSchema.AnyOf;
        }

        // Optional: hide plain Result (non-generic)
        else if (type == typeof(Result))
        {
            schema.Properties.Clear();
            schema.Type = "object";
        }
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var fluentAssembly = typeof(Result).Assembly;

        // Get all type names from the FluentResults assembly
        var fluentTypeNames = fluentAssembly.GetTypes()
                .SelectMany(t => new[] { t.Name, t.FullName ?? string.Empty })
                .Where(n => !string.IsNullOrEmpty(n)).ToHashSet();

        // Remove schemas related to FluentResults types
        context.SchemaRepository.Schemas
            .Where(entry => fluentTypeNames.Contains(entry.Key))
            .Select(entry => entry.Key)
            .ToList()
            .ForEach(key => context.SchemaRepository.Schemas.Remove(key));
    }

}