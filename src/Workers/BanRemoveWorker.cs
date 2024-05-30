using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Contracts;
using Services.Contracts.Contracts;

namespace Workers;

public class BanRemoveWorker : IHostedService, IDisposable
{
    private readonly ILoggerService _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private Task? _task;
    private readonly CancellationTokenSource _cts;

    public BanRemoveWorker(ILoggerService logger, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _cts = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _task = RemoveExpiredBans(_cts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_task == null)
            return;
        _logger.LogInfo("Stopping ban removal worker.");

        try
        {
            _cts.Cancel();
        }
        finally
        {
            _logger.LogInfo("Waiting for ban removal worker to finish.");
            await Task.WhenAny(_task, Task.Delay(-1, cancellationToken));
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        GC.SuppressFinalize(this);
    }

    private async Task RemoveExpiredBans(CancellationToken cancellationToken)
    {
        var delay = _configuration.GetValue<float>("ForumConfiguration:BanDelayHours");
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay((int)(delay * 60 * 60 * 1000), cancellationToken);
            try{
                using var scope = _serviceProvider.CreateScope();
                _logger.LogInfo("Checking for expired bans. ");
                var serviceManager = scope.ServiceProvider.GetRequiredService<IServiceManager>();
                await serviceManager.BanService.RevokeExpiredBans(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError($"Ban removal worker error! {exc.Message}");
            }        }
    }
}