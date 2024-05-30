using System.Globalization;
using System.Reflection;
using System.Text;
using Domain.Entities;
using Forum.Resources;
using Logging;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Repository.Contracts;
using Services;
using Services.Contracts;
using Services.Contracts.Contracts;
using Services.Services;

namespace Forum.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// INSECURE, USE ONLY IN DEVELOPMENT FOR TESTING
    /// Configure the CORS to allow all methods and domains to access.
    /// </summary>
    public static void ConfigureCorsAsDevelopment(this IServiceCollection services)
    {
        Console.WriteLine(Localization.CorsPolicyWarning);
        services.AddCors(options => options.AddPolicy("CorsPolicy", builder => builder
            .WithOrigins()
            .AllowAnyMethod()
            .AllowAnyHeader()));
    }

    /// <summary>
    /// Configures the CORS so that only the correct domain and methods/headers can access.
    /// </summary>
    /// todo make this conditional. If a valid URL is provided don't throw an exception.
    public static void ConfigureCorsAsDeployment(this IServiceCollection services) =>
        // Uncomment the following code and comment the throw statement to use the deployment policy.
        // services.AddCors(options => options.AddPolicy("CorsPolicy", builder => builder
        //     .WithOrigins("https://api.forum.ge//")
        //     .WithMethods("POST", "GET", "DELETE", "PATCH")
        //     .AllowAnyHeader()));
        throw new Exception("The CORS policy for the application in deployment is not configured. " +
                            "Please configure or use the development policy. " +
                            "Check ServiceExtensions.ConfigureCorsAsDeployment for details.");
    
    /// <summary>
    /// Configures the Logger Infrastructure from the LoggerService project.
    /// </summary>
    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerService, FileLogger>();
    
    /// <summary>
    /// Configures the HTTP Logging Middleware.
    /// </summary>
    public static void ConfigureHttpLoggingMiddleware(this IServiceCollection services) =>
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestHeaders |
                                    HttpLoggingFields.RequestBody |
                                    HttpLoggingFields.ResponseHeaders |
                                    HttpLoggingFields.ResponseBody;
        });
    
    /// <summary>
    /// Configures the Repository Manager from the Repository project. Used for the domain layer of the onion architecture.
    /// </summary>
    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();
    
    /// <summary>
    /// Configures the Infrastructure Manager from the Infrastructure project. Used for the service layer of the onion architecture.
    /// </summary>
    public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();
    
    /// <summary>
    /// Configures the Authentication Service DI, which has the logic for the JWT token.
    /// </summary>
    public static void ConfigureAuthenticationService(this IServiceCollection services) =>
        services.AddScoped<IAuthenticationService, AuthenticationService>();

    public static void ConfigureAuthorizationService(this IServiceCollection services) =>
        services.AddAuthorization();

    /// <summary>
    /// Configures the SQL Context for the MsSql database.
    /// </summary>
    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts =>
        {
            opts.UseSqlServer(Environment.GetEnvironmentVariable("SQLCONFIG") ??
                              throw new InvalidOperationException("The connection string is null."));
        });
    
    /// <summary>
    /// Configures the Configuration DI.
    /// </summary>
    public static void ConfigureConfiguration(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSingleton(configuration);

    /// <summary>
    /// Configures the Mapster configuration.
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureMapster(this IServiceCollection services) =>
        TypeAdapterConfig.GlobalSettings.Default.MapToConstructor(true);

    /// <summary>
    /// Configures the Identity.
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureIdentity(this IServiceCollection services) =>
        services.AddIdentity<User, IdentityRole>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();

    /// <summary>
    /// Configures the Swagger UI.
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureSwagger(this IServiceCollection services) =>
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Forum API", Version = "v1" });
            c.SwaggerDoc("v2", new OpenApiInfo { Title = "Forum API", Version = "v2" });
            
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                In = ParameterLocation.Header
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                    },
                    Array.Empty<string>()
                }
            });
            
            var xmlFile = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            
            c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            
            c.EnableAnnotations();
        });
    
    /// <summary>
    /// Configures the JWT. The secret key is stored in the environment variables.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void ConfigureJwt(this IServiceCollection services, IConfiguration
        configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = Environment.GetEnvironmentVariable("SECRET");
        services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                };
            });
    }
    
    /// <summary>
    /// Adds health checks to the application, including sql.
    /// </summary>
    public static void ConfigureHealthChecksForSql(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks().AddSqlServer(Environment.GetEnvironmentVariable("SQLCONFIG") ??
                 throw new InvalidOperationException("The connection string is null."));
    }
    
    /// <summary>
    /// Adds Localization to the application.
    /// </summary>
    public static void ConfigureLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("ka")
            };
            
            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new AcceptLanguageHeaderRequestCultureProvider(),
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider(),
            };
        });
    }
}