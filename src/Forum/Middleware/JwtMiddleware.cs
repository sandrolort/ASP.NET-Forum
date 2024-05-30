using Services.Contracts.Contracts;

namespace Forum.Middleware;

/// <summary>
/// Used to add the JWT token to the request headers if it is present in the cookies.
/// Used for Razor Pages.
/// </summary>
public static class JwtMiddleware
{
    /// <summary>
    /// Adds the JWT token to the request headers if it is present in the cookies.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance this method extends.</param>
    public static void UseJwtMiddleware(this WebApplication app) =>
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                await next(context);
                return;
            }
        
            var jwt = context.Request.Cookies["jwt"];

            if (jwt != null && !context.Request.Headers.ContainsKey("Authorization"))
                context.Request.Headers.Append("Authorization", $"Bearer {jwt}");

            await next(context);
        });
    
    public static void JwtBlacklistMiddleware(this WebApplication app) =>
        app.Use(async (context, next) =>
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                await next(context);
                return;
            }
            var jwt = context.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (IBanService.BlackList.ContainsKey(jwt))
            {
                context.Request.Headers.Remove("Authorization");
                context.Response.Cookies.Delete("refreshToken");
                context.Response.Cookies.Delete("jwt");
            }
            await next(context);
        });
}