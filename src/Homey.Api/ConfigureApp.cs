using Homey.Api.Modules;
using Serilog;

namespace Homey.Api;

public static class ConfigureApp
{
    public static void Configure(this WebApplication app)
    {
        // Should be the first
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();
        
        app.UseCors("AllowNuxtLocalhost");

        app.UseAuthentication();
        app.UseAuthorization();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseOutputCache();
        
        // Add endpoints
        app.MapEndpoints();
    }
}