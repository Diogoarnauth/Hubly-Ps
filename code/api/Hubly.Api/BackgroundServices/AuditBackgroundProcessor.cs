using Hubly.api.Infrastructure.Audit;
using Hubly.api.Services.Interfaces; 

namespace Hubly.api.BackgroundServices;
public class AuditBackgroundProcessor : BackgroundService
{
    private readonly AuditQueue _queue;
    private readonly IServiceProvider _serviceProvider;

    public AuditBackgroundProcessor(AuditQueue queue, IServiceProvider serviceProvider)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var entry in _queue.DequeueAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                
                await auditService.LogAction(entry.Action, entry.UserId, entry.CoWorkerId, entry.Path, entry.Payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro crítico ao processar log: {ex.Message}");
            }
        }
    }
}