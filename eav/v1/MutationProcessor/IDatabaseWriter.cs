using System.Threading;
using System.Threading.Tasks;

namespace MutationProcessor
{
    public interface IDatabaseWriter
    {
        Task<bool> Verify(CancellationToken cancellationToken);
        Task<bool> Append(Change change, CancellationToken cancellationToken);
    }
}