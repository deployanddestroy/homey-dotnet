using System.ComponentModel.DataAnnotations;

namespace Homey.Api.Common.Filters;

public class RequestValidationFilter<TRequest>(ILogger<RequestValidationFilter<TRequest>> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Get the name of the request, including the namespace (ex: Homey.Api.Modules.Homes.GetHome)
        var requestName = typeof(TRequest).FullName;
        
        logger.LogInformation($"Validating: {requestName}");
        
        // We need to get the request from context arguments, because it's not injected via DI automatically
        var request = context.Arguments.OfType<TRequest>().First();
        // I don't know how this could happen, but it's possible apparently since it's nullable above
        if (request is null) return await next(context);
        
        // Set up the context and the result list
        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();
    
        // return early if successful
        if (Validator.TryValidateObject(request, validationContext, validationResults, true))
            return await next(context);
        
        // otherwise return a failed request
        logger.LogWarning($"Validation failed: {requestName}");
        var errors = validationResults
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(
                grp => grp.Key, 
                grp => grp
                    .Select(r => r.ErrorMessage).ToArray());
        return TypedResults.ValidationProblem(errors!);
    }
}