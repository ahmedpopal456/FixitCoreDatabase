using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Fixit.Storage.Adapters.Cosmos
{
    public class CosmosDbAdapter : IClientDbAdapter
    {
        private CosmosClient _cosmosClient;

        public CosmosDbAdapter(CosmosClient cosmosClient)
        {
            this._cosmosClient = cosmosClient;
        }

        public async Task<IClientDbTableAdapter> CreateDatabaseAsync(string databaseId, CancellationToken cancellationToken = default)
        {
            return new CosmosDbTableAdapter(await _cosmosClient.CreateDatabaseAsync(databaseId, cancellationToken: cancellationToken));
        }

        public async Task<IClientDbTableAdapter> CreateDatabaseIfNotExistsAsync(string databaseId, CancellationToken cancellationToken = default)
        {
            return new CosmosDbTableAdapter(await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId, cancellationToken: cancellationToken));
        }

        public void Dispose()
        {
            _cosmosClient.Dispose();
        }

        public IClientDbTableAdapter GetDatabase(string databaseId)
        {
            return new CosmosDbTableAdapter(_cosmosClient.GetDatabase(databaseId));
        }
    }
}
