using Microsoft.AspNetCore.OutputCaching;

namespace Homey.Api.Modules.Pros;

public class DeletePro : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id}", Handle)
            .WithSummary("Deletes a professional")
            .WithRequestValidation<Request>();
    }
    
    public record Request(Guid id);
    
    public static async Task<Results<NoContent, NotFound>> Handle(
        [AsParameters] Request request,
        AppDbContext dbContext, 
        ClaimsPrincipal claimsPrincipal, 
        CancellationToken cancellationToken)
    {
        var deletedCount = await dbContext.Professionals
            .Where(h => h.Id == request.id && h.UserId == claimsPrincipal.GetUserId())
            .ExecuteDeleteAsync(cancellationToken);

        return deletedCount > 0 ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}