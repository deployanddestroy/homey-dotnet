using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.OutputCaching;

namespace Homey.Api.Modules.Pros;

public class AddPro : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", Handle)
            .WithSummary("Adds a new professional")
            .WithRequestValidation<Request>();
    }
    
    public record Request(
        [property: Required(AllowEmptyStrings = false), MaxLength(255)] string Name,
        [property: Required] Guid ProfessionalTypeId
        );

    public record Response(
        Guid Id,
        string Name,
        ProfessionalType? ProfessionalType);
    
    private static async Task<Results<Created<Response>, BadRequest>> Handle(
        Request request, 
        AppDbContext db, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var pro = new Professional
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            UserId = claimsPrincipal.GetUserId(),
            ProfessionalTypeId = request.ProfessionalTypeId
        };
        
        await db.Professionals.AddAsync(pro, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        // fetch the pro type
        var proType = await db.ProfessionalTypes
            .Where(p => p.Id == pro.ProfessionalTypeId)
            .SingleOrDefaultAsync(cancellationToken);
        
        //TODO: send an event out
        var response = new Response(pro.Id, pro.Name, proType);
        return TypedResults.Created(response.Id.ToString(), response);
    }
}