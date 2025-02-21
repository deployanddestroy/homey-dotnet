namespace Homey.Api.Modules.Homes;

public class DeleteHome : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id}", Handle)
            .WithSummary("Deletes a home")
            .WithRequestValidation<Request>();
    }

    public record Request(Guid id);

    public static async Task<Results<NoContent, NotFound>> Handle([AsParameters] Request request,
        AppDbContext dbContext, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var deletedCount = await dbContext.Homes
            .Where(h => h.Id == request.id && h.UserId == claimsPrincipal.GetUserId())
            .ExecuteDeleteAsync(cancellationToken);
        
        return deletedCount > 0 ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}