using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Fixit.Storage.Adapters.Cosmos.Internal
{
  internal class CosmosDatabaseAdapter : IDatabaseAdapter
  {
    private CosmosClient _cosmosClient;

    public CosmosDatabaseAdapter(CosmosClient cosmosClient)
    {
      _cosmosClient = cosmosClient;
    }

    public async Task<IDatabaseTableAdapter> CreateDatabaseIfNotExistsAsync(string databaseId, CancellationToken cancellationToken)
    {
      return new CosmosDatabaseTableAdapter(await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId, cancellationToken: cancellationToken));
    }

    public void Dispose()
    {
      _cosmosClient.Dispose();
    }

    public IDatabaseTableAdapter GetDatabase(string databaseId)
    {
      return new CosmosDatabaseTableAdapter(_cosmosClient.GetDatabase(databaseId));
    }
  }
}
