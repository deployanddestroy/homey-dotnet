namespace Homey.Api.Modules.Inventory;

public class GetInventoryItemById : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", Handle)
            .WithSummary("Gets an inventory item by Id")
            .WithRequestValidation<Request>();
    }
    
    private record Request(Guid Id);

    private record Response(
        Guid Id,
        Guid HomeId,
        Guid? RoomId,
        string Name,
        string? Description,
        string? SerialNumber,
        double? Price
        );
    
    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        [AsParameters] Request request, 
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var item = await db.InventoryItems
            .Where(i => i.Id == request.Id && i.UserId == claimsPrincipal.GetUserId())
            .Select(i => new Response(i.Id, 
                i.HomeId,
                i.RoomId,
                i.Name,
                i.Description,
                i.SerialNumber,
                i.Price))
            .SingleOrDefaultAsync(cancellationToken);
        
        return item == null ? TypedResults.NotFound() : TypedResults.Ok(item);
    }
}