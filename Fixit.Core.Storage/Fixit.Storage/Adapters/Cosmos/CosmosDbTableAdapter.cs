using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Fixit.Storage.Adapters.Cosmos
{
    class CosmosDbTableAdapter : IClientDbTableAdapter
    {
        private Database _database;

        public CosmosDbTableAdapter(Database database)
        {
            this._database = database;
        }

        public async Task<IClientDbTableEntityAdapter> CreateContainerAsync(string containerId, string partitionKeyPath, CancellationToken cancellationToken = default)
        {
            return new CosmosDbTableEntityAdapter(await _database.CreateContainerAsync(containerId, partitionKeyPath, cancellationToken: cancellationToken));
        }

        public async Task<IClientDbTableEntityAdapter> CreateContainerIfNotExistsAsync(string containerId, string partitionKeyPath, CancellationToken cancellationToken = default)
        {
            return new CosmosDbTableEntityAdapter(await _database.CreateContainerIfNotExistsAsync(containerId, partitionKeyPath, cancellationToken: cancellationToken));
        }

        public IClientDbTableEntityAdapter GetContainer(string containerId)
        {
            return new CosmosDbTableEntityAdapter(_database.GetContainer(containerId));
        }
    }
}
