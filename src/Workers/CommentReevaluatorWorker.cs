using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Contracts;
using Services.Contracts.Contracts;

namespace Workers;

public class CommentReevaluatorWorker: IHostedService, IDisposable
{
    private readonly ILoggerService _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private Task? _task;
    private readonly CancellationTokenSource _cts;

    public CommentReevaluatorWorker(ILoggerService logger, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _cts = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _task = ArchiveOldTopics(_cts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_task == null)
            return;
        _logger.LogInfo("Stopping topic archival worker.");

        try
        {
            _cts.Cancel();
        }
        finally
        {
            _logger.LogInfo("Waiting for topic archival worker to finish.");
            await Task.WhenAny(_task, Task.Delay(-1, cancellationToken));
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        GC.SuppressFinalize(this);
    }

    private async Task ArchiveOldTopics(CancellationToken cancellationToken)
    {
        var delay = _configuration.GetValue<float>("ForumConfiguration:CommentReevaluationDelayHours");
        try
        {
            await Task.Delay((int)(delay * 60 * 60 * 1000), cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var commentService = scope.ServiceProvider.GetRequiredService<IServiceManager>().CommentService;
                await commentService.ReevaluateCommentCount(cancellationToken);
            }
        }
        catch (Exception exc)
        {
            _logger.LogError($"Comment evaluation worker error! {exc.Message}");
        }
    }
}