using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Fixit.Core.DataContracts;
using Fixit.Core.Database.DataContracts.Documents;

namespace Fixit.Core.Database.Mediators
{
  public interface IDatabaseTableEntityMediator
  {
    /// <summary>
    /// Creates the given item in the database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="partitionKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CreateDocumentDto<T>> CreateItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase;

    /// <summary>
    /// Deletes the item with the specified itemId from the database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="itemId"></param>
    /// <param name="partitionKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OperationStatus> DeleteItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase;

    /// <summary>
    /// Retrieves the item with the specified itemId from the database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="itemId"></param>
    /// <param name="partitionKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DocumentDto<T>> GetItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase;

    /// <summary>
    /// Retrieves items from the database using the specified query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="continuationToken"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="queryRequestOptions"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<(DocumentCollectionDto<T> DocumentCollection, string ContinuationToken)> GetItemQueryableAsync<T>(string continuationToken, CancellationToken cancellationToken, Expression<Func<T, bool>> predicate, QueryRequestOptions queryRequestOptions = default) where T : DocumentBase;

    /// <summary>
    /// Retrieves items from the database using the specified query, by page
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pageNumber"></param>
    /// <param name="queryRequestOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<PagedDocumentCollectionDto<T>> GetItemQueryableByPageAsync<T>(int pageNumber, QueryRequestOptions queryRequestOptions, CancellationToken cancellationToken, Expression<Func<T, bool>> predicate) where T : DocumentBase;

    /// <summary>
    /// Insert or update the specified item in the database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="partitionKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OperationStatus> UpsertItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase;
  }
}
