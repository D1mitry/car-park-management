using FluentResults;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ParkingManagement.Api.Filters;

internal sealed class FluentResultOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check the actual return type of the action
        var returnType = GetReturnType(context);

        if (returnType == null)
            return;

        // Handle only Response<T>
        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = returnType.GetGenericArguments()[0];

            // Update the response schema for 200 OK
            if (operation.Responses.TryGetValue("200", out var response))
            {
                response.Content.Clear();

                var schema = context.SchemaGenerator.GenerateSchema(valueType, context.SchemaRepository);
                response.Content["application/json"] = new OpenApiMediaType
                {
                    Schema = schema
                };
            }
        }
    }

    private static Type? GetReturnType(OperationFilterContext context)
    {
        var returnType = context.MethodInfo.ReturnType;

        if (typeof(Task).IsAssignableFrom(returnType))
        {
            if (returnType.IsGenericType)
                return returnType.GetGenericArguments()[0];
            return null;
        }

        return returnType;
    }
}
