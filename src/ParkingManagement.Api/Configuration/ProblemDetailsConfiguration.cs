using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace ParkingManagement.Api.Configuration;

internal sealed class ProblemDetailsConfiguration(ExceptionErrorOptions exceptionsOptions) : IConfigureOptions<ProblemDetailsOptions>
{
    public void Configure(ProblemDetailsOptions options)
    {
        options.CustomizeProblemDetails = context =>
        {
            var exception = context.HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;
            if (exception is null)
                return;

            context.ProblemDetails.Detail = exception.Message;
            context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString("G");

            var exceptionErrorDetails = exceptionsOptions.GetErrorDetails(exception);

            if (!exceptionErrorDetails.HasValue)
                return;

            context.ProblemDetails.Title = ReasonPhrases.GetReasonPhrase(exceptionErrorDetails.Value.StatusCode);
            context.ProblemDetails.Status = exceptionErrorDetails.Value.StatusCode;
            context.ProblemDetails.Type = exceptionErrorDetails.Value.Type;
        };
    }
}
