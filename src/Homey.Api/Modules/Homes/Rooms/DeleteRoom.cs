namespace Homey.Api.Modules.Homes.Rooms;

public class DeleteRoom : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{homeId}/rooms/{roomId}", Handle)
            .WithSummary("Deletes a room from a home")
            .WithRequestValidation<Request>();
    }
    
    public record Request(Guid HomeId, Guid RoomId);

    public static async Task<Results<NoContent, NotFound>> Handle([AsParameters] Request request,
        AppDbContext dbContext, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var deletedCount = await dbContext.Rooms
            .Where(r => 
                r.Id == request.RoomId 
                && r.HomeId == request.HomeId 
                && r.UserId == claimsPrincipal.GetUserId())
            .ExecuteDeleteAsync(cancellationToken);
        
        return deletedCount > 0 ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}