using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using ParkingManagement.Api.Configuration;

namespace ParkingManagement.Api.Filters;

internal sealed class FluentResultsAsyncFilter(FluentResultsTransformer transformer) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {

        var traceId = System.Diagnostics.Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

        context.Result = context.Result switch
        {
            ObjectResult { Value: Result result }
                => transformer.Transform(result, traceId),

            ObjectResult { Value: var value } when value?.GetType().IsGenericType == true
                        && value.GetType().GetGenericTypeDefinition() == typeof(Result<>)
                => transformer.Transform((dynamic)value!, traceId),

            _ => context.Result,
        };

        await next();
    }
}

internal sealed class FluentResultsTransformer(FluentResultsErrorOptions options)
{
    public IActionResult Transform<T>(Result<T> result, string traceId)
        => result.ToActionResult(options, traceId);

    public IActionResult Transform(Result result, string traceId)
        => result.ToActionResult(options, traceId);
}

file static class ResultExtensions
{
    internal static IActionResult ToActionResult(this Result result, FluentResultsErrorOptions options, string traceId)
        => result.IsSuccess
            ? new OkResult()
            : result.ToProblemDetailsResponse(options, traceId);

    internal static IActionResult ToActionResult<T>(this Result<T> result, FluentResultsErrorOptions options, string traceId)
        => result.IsSuccess
            ? result.ToOkResponse()
            : result.ToResult().ToProblemDetailsResponse(options, traceId);

    private static IActionResult ToOkResponse<T>(this Result<T> result)
        => result.Value is null
            ? new OkResult()
            : new OkObjectResult(result.Value);

    private static ObjectResult ToProblemDetailsResponse(this Result result, FluentResultsErrorOptions options, string traceId)
    {
        IError error = result.Errors[0];
        var problemDetails = new ProblemDetails
        {
            Detail = string.Join("; ", result.Errors.Select(e => e.Message)),
            Extensions = {
                { "traceId", traceId},
                { "timestamp", DateTime.UtcNow.ToString("G") },
            }
        };

        (int StatusCode, string Type)? errorDetails = options.GetErrorDetails(error);

        if (errorDetails.HasValue)
        {
            problemDetails.Title = ReasonPhrases.GetReasonPhrase(errorDetails.Value.StatusCode);
            problemDetails.Status = errorDetails.Value.StatusCode;
            problemDetails.Type = errorDetails.Value.Type;
        }

        return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
    }
}


