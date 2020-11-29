using System;

using Fixit.Storage.Adapters;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Storage.Mediators.Internal
{
  internal class CosmosDbMediator : IClientDbMediator
  {
    private IClientDbAdapter _dbAdapter;

    public CosmosDbMediator(IClientDbAdapter dbAdapter)
    {
      _dbAdapter = dbAdapter;
    }

    public async Task<IClientDbTableMediator> CreateDatabaseAsync(string databaseId, CancellationToken cancellationToken)
    {
      if (string.IsNullOrWhiteSpace(databaseId))
      {
        throw new ArgumentNullException($"{nameof(CreateDatabaseAsync)} expects a valid value for {nameof(databaseId)}");
      }

      return new CosmosDbTableMediator(await _dbAdapter.CreateDatabaseIfNotExistsAsync(databaseId, cancellationToken));
    }

    public IClientDbTableMediator GetDatabase(string databaseId)
    {
      if (string.IsNullOrWhiteSpace(databaseId))
      {
        throw new ArgumentNullException($"{nameof(GetDatabase)} expects a valid value for {nameof(databaseId)}");
      }

      return new CosmosDbTableMediator(_dbAdapter.GetDatabase(databaseId));
    }
  }
}
