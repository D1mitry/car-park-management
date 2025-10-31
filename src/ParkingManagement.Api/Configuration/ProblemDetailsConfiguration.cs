using DomainBase;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using ParkingManagement.Application.Exceptions;

namespace ParkingManagement.Api.Configuration;

internal sealed class ProblemDetailsSetup : IConfigureOptions<ProblemDetailsOptions>
{
    private static readonly Dictionary<Type, int> ExceptionStatusCodeMapping = new()
    {
        { typeof(DomainValidationException), StatusCodes.Status400BadRequest },
        { typeof(DomainConflictException), StatusCodes.Status409Conflict },
        { typeof(ConflictException), StatusCodes.Status409Conflict},
    };

    private static readonly Dictionary<int, string> StatusCodeTypeMapping = new()
    {
        { StatusCodes.Status400BadRequest, "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1" },
        { StatusCodes.Status409Conflict, "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10" },
    };

    public void Configure(ProblemDetailsOptions options)
    {
        options.CustomizeProblemDetails = context =>
        {
            var exception = context.HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;
            if (exception is null)
                return;

            context.ProblemDetails.Detail = exception.Message;
            context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString("G");

            if (!ExceptionStatusCodeMapping.TryGetValue(exception.GetType(), out int statusCode))
                return;

            context.ProblemDetails.Title = ReasonPhrases.GetReasonPhrase(statusCode);
            context.ProblemDetails.Status = statusCode;
            context.ProblemDetails.Type = StatusCodeTypeMapping.GetValueOrDefault(statusCode);
        };
    }
}
