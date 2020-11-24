using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Storage.Adapters
{
    public interface IClientDbAdapter: IDisposable
    {
        Task<IClientDbTableAdapter> CreateDatabaseAsync(string databaseId, CancellationToken cancellationToken);

        Task<IClientDbTableAdapter> CreateDatabaseIfNotExistsAsync(string databaseId, CancellationToken cancellationToken);

        IClientDbTableAdapter GetDatabase(string databaseId);
    }
}
