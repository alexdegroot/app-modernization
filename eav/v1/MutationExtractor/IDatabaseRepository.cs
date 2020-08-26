using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MutationExtractor
{
    public interface IDatabaseRepository
    {
        Task<bool> Verify(CancellationToken cancellationToken);
        IAsyncEnumerable<Change> GetChanges(CancellationToken cancellationToken);
    }
}