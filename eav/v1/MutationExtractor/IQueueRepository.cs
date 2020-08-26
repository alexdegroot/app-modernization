using System.Threading;
using System.Threading.Tasks;

namespace MutationExtractor
{
    public interface IQueueRepository
    {
        Task<bool> Ensure(CancellationToken cancellationToken);
        Task AddMessage<T>(T message, CancellationToken cancellationToken);
    }
}