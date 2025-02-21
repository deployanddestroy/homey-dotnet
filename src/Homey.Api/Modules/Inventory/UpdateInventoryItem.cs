using System.ComponentModel.DataAnnotations;

namespace Homey.Api.Modules.Inventory;

public class UpdateInventoryItem : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id}", Handle)
            .WithSummary("Updates an inventory item")
            .WithRequestValidation<Request>();
    }
    
    public record Request(
        Guid? HomeId,
        Guid? RoomId,
        string? SerialNumber,
        [property: MaxLength(100)] string Name,
        string? Description,
        double? Price);
    
    public record Response(
        Guid Id,
        Guid HomeId,
        Guid? RoomId,
        string? SerialNumber,
        string Name,
        string? Description,
        double? Price);
    
    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        Guid id,
        Request request, 
        AppDbContext db, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var item = await db.InventoryItems
            .SingleOrDefaultAsync(
                i => i.Id == id 
                     && i.UserId == claimsPrincipal.GetUserId(), 
                cancellationToken);
        if (item is null) return TypedResults.NotFound();
        
        item.HomeId = request.HomeId ?? item.HomeId;
        item.RoomId = request.RoomId;
        item.SerialNumber = request.SerialNumber;
        item.Name = request.Name;
        item.Description = request.Description;
        item.Price = request.Price;
        
        await db.SaveChangesAsync(cancellationToken);
        
        //TODO: send an event out
        
        var response = new Response(
            item.Id,
            item.HomeId,
            item.RoomId,
            item.SerialNumber,
            item.Name,
            item.Description,
            item.Price);
        
        return TypedResults.Ok(response);
    }
}