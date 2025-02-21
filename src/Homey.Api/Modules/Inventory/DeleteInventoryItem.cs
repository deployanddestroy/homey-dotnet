namespace Homey.Api.Modules.Inventory;

public class DeleteInventoryItem : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id}", Handle)
            .WithSummary("Deletes a inventory item")
            .WithRequestValidation<Request>();
    }
    
    public record Request(Guid id);
    
    public static async Task<Results<NoContent, NotFound>> Handle([AsParameters] Request request,
        AppDbContext dbContext, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var deletedCount = await dbContext.InventoryItems
            .Where(i => i.Id == request.id && i.UserId == claimsPrincipal.GetUserId())
            .ExecuteDeleteAsync(cancellationToken);
        
        return deletedCount > 0 ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}