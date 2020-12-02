using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Fixit.Database.Adapters
{
  public interface IDatabaseTableEntityAdapter
  {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="partitionKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> CreateItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="itemId"></param>
    /// <param name="partitionKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<HttpStatusCode> DeleteItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="continuationToken"></param>
    /// <param name="queryRequestOptions"></param>
    /// <returns></returns>
    IOrderedQueryable<T> GetItemLinqQueryable<T>(string continuationToken, QueryRequestOptions queryRequestOptions);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queryDefinition"></param>
    /// <param name="continuationToken"></param>
    /// <param name="queryRequestOptions"></param>
    /// <returns></returns>
    FeedIterator<T> GetItemQueryIterator<T>(QueryDefinition queryDefinition, string continuationToken, QueryRequestOptions queryRequestOptions);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="itemId"></param>
    /// <param name="partitionKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> ReadItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="itemId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> ReplaceItemAsync<T>(T item, string itemId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="partitionKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<HttpStatusCode> UpsertItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken);
  }
}
