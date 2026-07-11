using System.Threading.Channels;
namespace Hubly.api.Infrastructure.Audit;

public class AuditQueue
{
    private readonly Channel<AuditEntry> _queue;

    public AuditQueue(int capacity = 1000)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<AuditEntry>(options);
    }

    public async ValueTask EnqueueAsync(AuditEntry entry) => await _queue.Writer.WriteAsync(entry);

    public IAsyncEnumerable<AuditEntry> DequeueAllAsync(CancellationToken ct) => _queue.Reader.ReadAllAsync(ct);
}