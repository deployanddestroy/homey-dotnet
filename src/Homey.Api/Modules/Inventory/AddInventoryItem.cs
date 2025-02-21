using System.ComponentModel.DataAnnotations;

namespace Homey.Api.Modules.Inventory;

public class AddInventoryItem : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", Handle)
            .WithSummary("Adds a new item to the inventory")
            .WithRequestValidation<Request>();
    }
    
    public record Request(
        [property: Required(AllowEmptyStrings = false), MaxLength(255)] string Name,
        [property: MaxLength(500)] string? Description,
        [property: MaxLength(50)] string? SerialNumber,
        [property: Range(0, int.MaxValue)] double? Price,
        [property: Required, NotEmptyGuid] Guid HomeId,
        Guid? RoomId 
    );

    public record Response(
        Guid Id,
        Guid HomeId,
        Guid? RoomId,
        string Name,
        string? Description,
        double? Price,
        string? SerialNumber);
    
    private static async Task<Results<Created<Response>, BadRequest>> Handle(
        Request request, 
        AppDbContext db, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var newItem = new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            SerialNumber = request.SerialNumber,
            Price = request.Price,
            HomeId = request.HomeId,
            RoomId = request.RoomId,
            UserId = claimsPrincipal.GetUserId()
        };
        
        await db.InventoryItems.AddAsync(newItem, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        //TODO: send an event out
        
        var response = new Response(
            newItem.Id, 
            newItem.HomeId, 
            newItem.RoomId, 
            newItem.Name, 
            newItem.Description, 
            newItem.Price, 
            newItem.SerialNumber);
        
        return TypedResults.Created(response.Id.ToString(), response);
    }
}