using Serilog;
using Serilog.Events;

namespace Forum.Extensions;

public static class HostExtensions
{
    public static void ConfigureSerilogAsILogger(this ConfigureHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((ctx, lc) =>
        {
            lc.WriteTo.File(ctx.Configuration["Logging:FilePath"]!, LogEventLevel.Warning)
              .WriteTo.File(ctx.Configuration["Logging:FilePathDump"]!);
        });
    }
}