using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Fixit.Storage.Adapters.Internal
{
  internal class CosmosDbTableEntityAdapter : IClientDbTableEntityAdapter
  {
    private Container _container;

    public CosmosDbTableEntityAdapter(Container container)
    {
      _container = container;
    }

    public async Task<T> CreateItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken)
    {
      return await _container.CreateItemAsync(item, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
    }

    public async Task<HttpStatusCode> DeleteItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken)
    {
      return (await _container.DeleteItemAsync<T>(itemId, new PartitionKey(partitionKey), cancellationToken: cancellationToken)).StatusCode;
    }

    public IOrderedQueryable<T> GetItemLinqQueryable<T>(string continuationToken, QueryRequestOptions queryRequestOptions)
    {
      return _container.GetItemLinqQueryable<T>(continuationToken: continuationToken, requestOptions: queryRequestOptions);
    }

    public FeedIterator<T> GetItemQueryIterator<T>(QueryDefinition queryDefinition, string continuationToken, QueryRequestOptions queryRequestOptions)
    {
      return _container.GetItemQueryIterator<T>(queryDefinition, continuationToken, queryRequestOptions);
    }

    public async Task<T> ReadItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken)
    {
      return await _container.ReadItemAsync<T>(itemId, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
    }

    public async Task<T> ReplaceItemAsync<T>(T item, string itemId, CancellationToken cancellationToken)
    {
      return await _container.ReplaceItemAsync(item, itemId, cancellationToken: cancellationToken);
    }

    public async Task<HttpStatusCode> UpsertItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken)
    {
      return (await _container.UpsertItemAsync(item, new PartitionKey(partitionKey), cancellationToken: cancellationToken)).StatusCode;
    }
  }
}
