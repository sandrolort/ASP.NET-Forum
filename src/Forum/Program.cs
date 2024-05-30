using FluentValidation;
using Forum.Extensions;
using Forum.Middleware;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Services.Contracts.Contracts;
using Web.Middleware;
using Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddViewLocalization();
var services = builder.Services;
services.ConfigureRepositoryManager();
services.ConfigureServiceManager();

services.ConfigureSqlContext(builder.Configuration);
services.ConfigureConfiguration(builder.Configuration);

services.ConfigureMapster();
services.ConfigureSwagger();

builder.Services.AddValidatorsFromAssembly(typeof(Validators.AssemblyReference).Assembly);

services.ConfigureLoggerService();
builder.Host.ConfigureSerilogAsILogger();
// if(builder.Environment.IsDevelopment())
//     services.ConfigureHttpLoggingMiddleware();

services.ConfigureIdentity();
services.ConfigureJwt(builder.Configuration);
services.ConfigureAuthenticationService();
services.ConfigureAuthorizationService();

services.AddHostedService<BanRemoveWorker>(provider => new BanRemoveWorker(
    provider.GetRequiredService<ILoggerService>(),
    provider.GetRequiredService<IConfiguration>(),
    provider));

services.AddHostedService<TopicArchivalWorker>(provider => new TopicArchivalWorker(
    provider.GetRequiredService<ILoggerService>(),
    provider.GetRequiredService<IConfiguration>(),
    provider));

services.AddHostedService<CommentReevaluatorWorker>(provider => new CommentReevaluatorWorker(
    provider.GetRequiredService<ILoggerService>(),
    provider.GetRequiredService<IConfiguration>(),
    provider));

if (builder.Environment.IsDevelopment())
    services.ConfigureCorsAsDevelopment();
else 
    services.ConfigureCorsAsDeployment();

services.ConfigureLocalization();

services.AddRazorPages()
    .AddViewLocalization();
services.AddControllers()
    .AddApplicationPart(typeof(Controllers.AssemblyReference).Assembly);

services.ConfigureHealthChecksForSql(builder.Configuration);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerService>();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseHttpLogging();

app.UseHealthCheckMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s=>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "Forum v1");
        s.SwaggerEndpoint("/swagger/v2/swagger.json", "Forum v2");
    });
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "../Web/wwwroot"))
    }
);

app.UseStatusCodeResponsesMiddleware();

app.UseJwtMiddleware();
app.JwtBlacklistMiddleware();

app.ConfigureExceptionHandler(logger);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Index}/{action=Index}/{id?}");
});

app.Run();