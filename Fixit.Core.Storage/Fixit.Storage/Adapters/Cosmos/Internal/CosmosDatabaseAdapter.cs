using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Fixit.Storage.Adapters.Cosmos.Internal
{
  internal class CosmosDatabaseAdapter : IDatabaseAdapter
  {
    private CosmosClient _cosmosClient;
    private bool _disposed = false;

    public CosmosDatabaseAdapter(CosmosClient cosmosClient)
    {
      _cosmosClient = cosmosClient ?? throw new ArgumentNullException($"{nameof(CosmosDatabaseAdapter)} expects a value for {nameof(cosmosClient)}... null argument was provided");
    }

    ~CosmosDatabaseAdapter() => Dispose(false);

    public async Task<IDatabaseTableAdapter> CreateDatabaseIfNotExistsAsync(string databaseId, CancellationToken cancellationToken)
    {
      return new CosmosDatabaseTableAdapter(await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId, cancellationToken: cancellationToken));
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_disposed)
      {
        return;
      }

      if (disposing)
      {
        _cosmosClient.Dispose();
      }

      _disposed = true;
    }

    public IDatabaseTableAdapter GetDatabase(string databaseId)
    {
      return new CosmosDatabaseTableAdapter(_cosmosClient.GetDatabase(databaseId));
    }
  }
}
