using System.ComponentModel.DataAnnotations;

namespace Homey.Api.Modules.Homes;

public class AddHome : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", Handle)
            .WithSummary("Adds a new home")
            .WithRequestValidation<Request>();
    }

    public record Request([property: Required(AllowEmptyStrings = false), MaxLength(255)]string Name);

    public record Response(
        Guid Id,
        string Name);
    
    private static async Task<Results<Created<Response>, BadRequest>> Handle(
        Request request, 
        AppDbContext db, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var home = new Home
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            UserId = claimsPrincipal.GetUserId()
        };
        
        await db.Homes.AddAsync(home, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        var response = new Response(home.Id, home.Name);
        return TypedResults.Created(response.Id.ToString(), response);
    }
}