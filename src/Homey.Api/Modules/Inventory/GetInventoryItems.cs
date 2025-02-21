using System.ComponentModel.DataAnnotations;

namespace Homey.Api.Modules.Inventory;

public class GetInventoryItems : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", Handle)
            .WithSummary("Gets inventory items")
            .WithRequestValidation<Request>();
    }
    
    public record Request(
        [property: Range(1, int.MaxValue)]int? Page, 
        [property: Range(1, 100)]int? PageSize) : IPagedRequest;

    public record Response(
        Guid Id,
        string Name,
        string? Description,
        double? Price,
        string? SerialNumber,
        Guid HomeId,
        Guid? RoomId
        );
    
    private static async Task<PagedList<Response>> Handle(
        Guid? HomeId,
        Guid? RoomId,
        [AsParameters] Request request, 
        ClaimsPrincipal claimsPrincipal, 
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        return await db.InventoryItems
            .Where(i => i.UserId == claimsPrincipal.GetUserId())
            .Where(i => i.HomeId == HomeId || HomeId == null)
            .Where(i => i.RoomId == RoomId || RoomId == null)
            .Select(i => new Response(
                i.Id, 
                i.Name,
                i.Description,
                i.Price,
                i.SerialNumber,
                i.HomeId,
                i.RoomId
                ))
            .ToPagedListAsync(request, cancellationToken);
    }
}