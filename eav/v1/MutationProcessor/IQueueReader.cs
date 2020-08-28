using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MutationProcessor
{
    public interface IQueueReader
    {
        Task<bool> Ensure(CancellationToken cancellationToken);
        IAsyncEnumerable<Change> GetChanges(CancellationToken stoppingToken);
    }
}