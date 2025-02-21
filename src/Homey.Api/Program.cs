global using Homey.Api;
global using Homey.Api.Common;
global using Homey.Api.Common.Filters;
global using Homey.Api.Common.Extensions;
global using Homey.Api.Common.Configuration;
global using Homey.Data;
global using Homey.Data.Models;

global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.Extensions.Options;
global using Microsoft.EntityFrameworkCore;
global using System.Security.Claims;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting Homey!");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddAllServices();
    
    await using var app = builder.Build();
    
    app.Configure();
    
    await app.RunAsync();
    
    Log.Information("Stopping Homey!");
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

