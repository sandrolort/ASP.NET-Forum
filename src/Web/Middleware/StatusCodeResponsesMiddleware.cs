using System.Net;

namespace Web.Middleware;

public static class StatusCodeResponsesMiddleware
{
    public static void UseStatusCodeResponsesMiddleware(this WebApplication app)
    {
        app.UseStatusCodePages(context =>
        {
            var response = context.HttpContext.Response;

            HttpStatusCode statusCode = (HttpStatusCode)response.StatusCode;

            if (context.HttpContext.Request.Path.StartsWithSegments("/api"))
            {
                return Task.CompletedTask;
            }

            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    response.Redirect("/Login");
                    break;
                case HttpStatusCode.Forbidden:
                    response.Redirect($"/Error/403");
                    break;
                case HttpStatusCode.NotFound:
                    response.Redirect($"/Error/404");
                    break;
                case HttpStatusCode.InternalServerError:
                    response.Redirect($"/Error/500");
                    break;
            }

            return Task.CompletedTask;
        });
    }
}