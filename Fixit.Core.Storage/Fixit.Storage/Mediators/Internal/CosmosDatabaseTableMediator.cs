using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Storage.Adapters;

namespace Fixit.Storage.Mediators.Internal
{
  internal class CosmosDatabaseTableMediator : IDatabaseTableMediator
  {
    private IDatabaseTableAdapter _databaseTableAdapter;

    public CosmosDatabaseTableMediator(IDatabaseTableAdapter databaseTableAdapter)
    {
      _databaseTableAdapter = databaseTableAdapter;
    }

    public async Task<IDatabaseTableEntityMediator> CreateContainerAsync(string containerId, string partitionKeyPath, CancellationToken cancellationToken)
    {
      if (string.IsNullOrWhiteSpace(containerId))
      {
        throw new ArgumentNullException($"{nameof(CreateContainerAsync)} expects a valid value for {nameof(containerId)}");
      }
      if (string.IsNullOrWhiteSpace(partitionKeyPath))
      {
        throw new ArgumentNullException($"{nameof(CreateContainerAsync)} expects a valid value for {nameof(partitionKeyPath)}");
      }

      return new CosmosDatabaseTableEntityMediator(await _databaseTableAdapter.CreateContainerIfNotExistsAsync(containerId, partitionKeyPath, cancellationToken));
    }

    public IDatabaseTableEntityMediator GetContainer(string containerId)
    {
      if (string.IsNullOrWhiteSpace(containerId))
      {
        throw new ArgumentNullException($"{nameof(GetContainer)} expects a valid value for {nameof(containerId)}");
      }

      return new CosmosDatabaseTableEntityMediator(_databaseTableAdapter.GetContainer(containerId));
    }
  }
}
