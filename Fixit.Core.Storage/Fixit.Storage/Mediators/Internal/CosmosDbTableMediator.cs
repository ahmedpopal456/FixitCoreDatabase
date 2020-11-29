using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Storage.Adapters;

namespace Fixit.Storage.Mediators.Internal
{
  internal class CosmosDbTableMediator : IClientDbTableMediator
  {
    private IClientDbTableAdapter _dbTableAdapter;

    public CosmosDbTableMediator(IClientDbTableAdapter dbTableAdapter)
    {
      _dbTableAdapter = dbTableAdapter;
    }

    public async Task<IClientDbTableEntityMediator> CreateContainerAsync(string containerId, string partitionKeyPath, CancellationToken cancellationToken)
    {
      if (string.IsNullOrWhiteSpace(containerId))
      {
        throw new ArgumentNullException($"{nameof(CreateContainerAsync)} expects a valid value for {nameof(containerId)}");
      }
      if (string.IsNullOrWhiteSpace(partitionKeyPath))
      {
        throw new ArgumentNullException($"{nameof(CreateContainerAsync)} expects a valid value for {nameof(partitionKeyPath)}");
      }

      return new CosmosDbTableEntityMediator(await _dbTableAdapter.CreateContainerIfNotExistsAsync(containerId, partitionKeyPath, cancellationToken));
    }

    public IClientDbTableEntityMediator GetContainer(string containerId)
    {
      if (string.IsNullOrWhiteSpace(containerId))
      {
        throw new ArgumentNullException($"{nameof(GetContainer)} expects a valid value for {nameof(containerId)}");
      }

      return new CosmosDbTableEntityMediator(_dbTableAdapter.GetContainer(containerId));
    }
  }
}
