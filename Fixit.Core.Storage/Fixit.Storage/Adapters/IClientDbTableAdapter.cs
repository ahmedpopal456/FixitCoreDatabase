using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Storage.Adapters
{
    public interface IClientDbTableAdapter
    {
        Task<IClientDbTableEntityAdapter> CreateContainerAsync(string containerId, string partitionKeyPath, CancellationToken cancellationToken);

        Task<IClientDbTableEntityAdapter> CreateContainerIfNotExistsAsync(string containerId, string partitionKeyPath, CancellationToken cancellationToken);

        IClientDbTableEntityAdapter GetContainer(string containerId);
    }
}
