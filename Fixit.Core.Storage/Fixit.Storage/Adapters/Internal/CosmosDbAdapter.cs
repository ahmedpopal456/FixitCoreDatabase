using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Fixit.Storage.Adapters.Internal
{
  internal class CosmosDbAdapter : IClientDbAdapter
  {
    private CosmosClient _cosmosClient;

    public CosmosDbAdapter(CosmosClient cosmosClient)
    {
      _cosmosClient = cosmosClient;
    }

    public async Task<IClientDbTableAdapter> CreateDatabaseIfNotExistsAsync(string databaseId, CancellationToken cancellationToken)
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
