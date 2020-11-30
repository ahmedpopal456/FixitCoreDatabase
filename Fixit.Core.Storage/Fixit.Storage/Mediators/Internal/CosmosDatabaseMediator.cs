using System;

using Fixit.Storage.Adapters;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Storage.Mediators.Internal
{
  internal class CosmosDatabaseMediator : IDatabaseMediator
  {
    private IDatabaseAdapter _databaseAdapter;

    public CosmosDatabaseMediator(IDatabaseAdapter databaseAdapter)
    {
      _databaseAdapter = databaseAdapter;
    }

    public async Task<IDatabaseTableMediator> CreateDatabaseAsync(string databaseId, CancellationToken cancellationToken)
    {
      if (string.IsNullOrWhiteSpace(databaseId))
      {
        throw new ArgumentNullException($"{nameof(CreateDatabaseAsync)} expects a valid value for {nameof(databaseId)}");
      }

      return new CosmosDatabaseTableMediator(await _databaseAdapter.CreateDatabaseIfNotExistsAsync(databaseId, cancellationToken));
    }

    public IDatabaseTableMediator GetDatabase(string databaseId)
    {
      if (string.IsNullOrWhiteSpace(databaseId))
      {
        throw new ArgumentNullException($"{nameof(GetDatabase)} expects a valid value for {nameof(databaseId)}");
      }

      return new CosmosDatabaseTableMediator(_databaseAdapter.GetDatabase(databaseId));
    }
  }
}
