using System.ComponentModel.DataAnnotations;

namespace Homey.Api.Modules.Homes.Rooms;

public class UpdateRoom : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{homeId}/rooms/{roomId}", Handle)
            .WithSummary("Updates a room in a home")
            .WithRequestValidation<Request>();
    }
    
    public record Request(
        [property: Required(AllowEmptyStrings = false), MaxLength(255)]string Name
    );

    private static async Task<Results<Ok, NotFound>> Handle(
        Guid homeId,
        Guid roomId,
        Request request, 
        AppDbContext db, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var room = await db.Rooms
            .SingleOrDefaultAsync(
                r => r.Id == roomId 
                     && r.HomeId == homeId
                && r.UserId == claimsPrincipal.GetUserId(), 
                cancellationToken);
        if (room is null) return TypedResults.NotFound();

        room.Name = request.Name;
        
        await db.SaveChangesAsync(cancellationToken);
        
        //TODO: send an event out

        return TypedResults.Ok();
    }
}