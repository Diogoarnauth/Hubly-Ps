using System.Threading.Channels;
namespace Hubly.api.Infrastructure.Audit;

public record AuditLogEntry(string Action, int? UserId, int? CoWorkerId, string Path, object Payload);

public class AuditQueue
{
    private readonly Channel<AuditLogEntry> _queue;

    public AuditQueue(int capacity = 1000)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait 
        };
        _queue = Channel.CreateBounded<AuditLogEntry>(options);
    }

    public async ValueTask EnqueueAsync(AuditLogEntry entry) => await _queue.Writer.WriteAsync(entry);
    
    public IAsyncEnumerable<AuditLogEntry> DequeueAllAsync(CancellationToken ct) => _queue.Reader.ReadAllAsync(ct);
}