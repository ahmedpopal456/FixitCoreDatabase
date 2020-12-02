using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Database.Adapters.Cosmos.Internal
{
  internal class CosmosDatabaseTableAdapter : IDatabaseTableAdapter
  {
    private Microsoft.Azure.Cosmos.Database _database;

    public CosmosDatabaseTableAdapter(Microsoft.Azure.Cosmos.Database database)
    {
      _database = database ?? throw new ArgumentNullException($"{nameof(CosmosDatabaseTableAdapter)} expects a value for {nameof(database)}... null argument was provided");
    }

    public async Task<IDatabaseTableEntityAdapter> CreateContainerIfNotExistsAsync(string containerId, string partitionKeyPath, CancellationToken cancellationToken)
    {
      return new CosmosDatabaseTableEntityAdapter(await _database.CreateContainerIfNotExistsAsync(containerId, partitionKeyPath, cancellationToken: cancellationToken));
    }

    public IDatabaseTableEntityAdapter GetContainer(string containerId)
    {
      return new CosmosDatabaseTableEntityAdapter(_database.GetContainer(containerId));
    }
  }
}
