using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Fixit.Storage.Adapters
{
    public interface IClientDbTableEntityAdapter
    {
        Task<T> CreateItemAsync<T>(T item, CancellationToken cancellationToken);

        Task<HttpStatusCode> DeleteItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken);

        IOrderedQueryable<T> GetItemLinqQueryable<T>(string continuationToken, QueryRequestOptions queryRequestOptions);

        FeedIterator<T> GetItemQueryIterator<T>(QueryDefinition queryDefinition, string continuationToken, QueryRequestOptions queryRequestOptions);

        Task<T> ReadItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken);

        Task<T> ReplaceItemAsync<T>(T item, string itemId, CancellationToken cancellationToken);

        Task<T> UpsertItemAsync<T>(T item, CancellationToken cancellationToken);
    }
}
