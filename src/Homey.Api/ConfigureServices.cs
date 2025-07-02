using System.Threading.Channels;
using Homey.Api.Modules.Email;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Homey.Api;

public static class ConfigureServices
{
    /// <summary>
    /// Adds all the services required for the app to function
    /// </summary>
    /// <param name="builder">Instance of WebApplicationBuilder</param>
    public static void AddAllServices(this WebApplicationBuilder builder)
    {
        builder.AddDatabase();
        builder.AddAuthentication();
        builder.AddExceptionHandling();
        builder.AddBackgroundServices();
        builder.AddChannels();
        builder.AddEmail();
        
        // Additional services which do not require their own extension methods
        builder.Services.AddApiVersioning();
        builder.Services.AddOpenApi();
        builder.Services.AddOutputCache(options =>
        {
            options.DefaultExpirationTimeSpan = TimeSpan.FromMinutes(10); // Set to 10 minutes.
        });
        builder.Services.AddSerilog((services, config) => config
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console());
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowNuxtLocalhost", policy =>
            {
                policy
                    .WithOrigins("http://localhost:3000", "https://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        builder.Services.AddHealthChecks();
    }
    
    /// <summary>
    /// Adds the AppDbContext with Postgres driver.
    /// </summary>
    /// <param name="builder">Instance of WebApplicationBuilder</param>
    /// <exception cref="ArgumentNullException">Thrown when the DefaultConnection connection string is empty</exception>
    private static void AddDatabase(this WebApplicationBuilder builder)
    {
        /*
         * Pull the connection string out of configuration, if null, throw
         * Create the DbContext with Postgres
         */
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
        builder.Services.AddDbContext<AppDbContext>(x => x.UseNpgsql(connectionString));
    }

    /// <summary>
    /// Add a global exception handler and the ProblemDetails scaffolding
    /// </summary>
    /// <param name="builder">Instance of WebApplicationBuilder</param>
    private static void AddExceptionHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
    }
    
    /// <summary>
    /// Adds authentication and authorization related services
    /// </summary>
    /// <param name="builder">Instance of WebApplicationBuilder</param>
    private static void AddAuthentication(this WebApplicationBuilder builder)
    {
        // Add IdentityCore using EntityFrameworkStores
        builder.Services.AddIdentityApiEndpoints<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(2);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<AppDbContext>();
        
        /*
         * Below, we're adding Microsoft's new authentication services. It's important to note
         * that this is not actually a JWT. It's stateless, but it can't be inspected in jwt.io
         */
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorizationBuilder();
    }
    
    /// <summary>
    /// Sets up background services for the application
    /// </summary>
    /// <param name="builder">Instance of WebApplicationBuilder</param>
    private static void AddBackgroundServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<EmailProcessor>();
    }
    
    /// <summary>
    /// Sets up channels for the application
    /// </summary>
    /// <param name="builder">Instance of WebApplicationBuilder</param>
    private static void AddChannels(this WebApplicationBuilder builder)
    {
        // We're setting up a singleton in-memory channel and initializing it as
        // unbounded. This means the sky (read: RAM) is the limit.
        builder.Services.AddSingleton<Channel<EmailMessage>>(
            _ => Channel.CreateUnbounded<EmailMessage>(new UnboundedChannelOptions
            {
                SingleReader = true, // Only one thing is allowed to read from this channel at a time
                SingleWriter = false, // Multiple places in the code can write into this channel (default: false)
                AllowSynchronousContinuations = false // Only async code can be executed on the channel
            }));
    }

    private static void AddEmail(this WebApplicationBuilder builder)
    {
        var smtpSettingsSection = builder.Environment.IsDevelopment() ? "SmtpOptions:Development" : "SmtpOptions:Production";
        builder.Services.AddOptions<SmtpOptions>()
            .BindConfiguration(smtpSettingsSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        builder.Services.AddTransient<IEmailSender<AppUser>, HomeyEmailSender<AppUser>>();
    }
}