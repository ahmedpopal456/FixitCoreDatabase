using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.Database.Adapters;

namespace Fixit.Core.Database.Mediators.Cosmos.Internal
{
  internal class CosmosDatabaseMediator : IDatabaseMediator
  {
    private IDatabaseAdapter _databaseAdapter;

    public CosmosDatabaseMediator(IDatabaseAdapter databaseAdapter)
    {
      _databaseAdapter = databaseAdapter ?? throw new ArgumentNullException($"{nameof(CosmosDatabaseMediator)} expects a value for {nameof(databaseAdapter)}... null argument was provided");
    }

    public async Task<IDatabaseTableMediator> CreateDatabaseAsync(string databaseId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

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
