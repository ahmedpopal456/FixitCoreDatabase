using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.Database.Adapters;
using Fixit.Core.Database.Adapters.Cosmos;
using Fixit.Core.DataContracts;
using Fixit.Core.Database.DataContracts.Documents;
using Microsoft.Azure.Cosmos;

[assembly: InternalsVisibleTo("Fixit.Core.Database.UnitTests")]
namespace Fixit.Core.Database.Mediators.Cosmos.Internal
{
  internal class CosmosDatabaseTableEntityMediator : IDatabaseTableEntityMediator
  {
    private IDatabaseTableEntityAdapter _databaseTableEntityAdapter;
    private ICosmosLinqQueryAdapter _cosmosLinqQueryAdapter;

    public CosmosDatabaseTableEntityMediator(IDatabaseTableEntityAdapter databaseTableEntityAdapter, ICosmosLinqQueryAdapter cosmosLinqQueryAdapter = null)
    {
      _databaseTableEntityAdapter = databaseTableEntityAdapter ?? throw new ArgumentNullException($"{nameof(CosmosDatabaseTableEntityMediator)} expects a value for {nameof(databaseTableEntityAdapter)}... null argument was provided");
      if (cosmosLinqQueryAdapter == null)
      {
        cosmosLinqQueryAdapter = new CosmosLinqQueryAdapter();
      }
      _cosmosLinqQueryAdapter = cosmosLinqQueryAdapter;
    }

    public async Task<CreateDocumentDto<T>> CreateItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();
      CreateDocumentDto<T> resultCreateDocument = new CreateDocumentDto<T>() { IsOperationSuccessful = true };

      if (string.IsNullOrWhiteSpace(partitionKey))
      {
        throw new ArgumentNullException($"{nameof(CreateItemAsync)} expects a valid value for {nameof(partitionKey)}");
      }

      try
      {
        item.id ??= Guid.NewGuid().ToString();
        item.EntityId = partitionKey;

        resultCreateDocument.Document = await _databaseTableEntityAdapter.CreateItemAsync(item, partitionKey, cancellationToken);
      }
      catch (Exception exception)
      {
        resultCreateDocument.OperationException = exception;
        resultCreateDocument.IsOperationSuccessful = false;
      }
      return resultCreateDocument;
    }

    public async Task<OperationStatus> DeleteItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();
      OperationStatus resultStatus = new OperationStatus();

      if (string.IsNullOrWhiteSpace(itemId))
      {
        throw new ArgumentNullException($"{nameof(DeleteItemAsync)} expects a valid value for {nameof(itemId)}");
      }
      if (string.IsNullOrWhiteSpace(partitionKey))
      {
        throw new ArgumentNullException($"{nameof(DeleteItemAsync)} expects a valid value for {nameof(partitionKey)}");
      }

      try
      {
        HttpStatusCode statusCode = await _databaseTableEntityAdapter.DeleteItemAsync<T>(itemId, partitionKey, cancellationToken);

        if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.NoContent)
        {
          resultStatus.IsOperationSuccessful = true;
        }
        resultStatus.OperationMessage = statusCode.ToString();
      }
      catch (Exception exception)
      {
        resultStatus.OperationException = exception;
        resultStatus.IsOperationSuccessful = false;
      }

      return resultStatus;
    }

    public async Task<DocumentDto<T>> GetItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();
      DocumentDto<T> document = new DocumentDto<T>() { IsOperationSuccessful = true };

      if (string.IsNullOrWhiteSpace(itemId))
      {
        throw new ArgumentNullException($"{nameof(GetItemAsync)} expects a valid value for {nameof(itemId)}");
      }
      if (string.IsNullOrWhiteSpace(partitionKey))
      {
        throw new ArgumentNullException($"{nameof(GetItemAsync)} expects a valid value for {nameof(partitionKey)}");
      }

      try
      {
        document.Document = await _databaseTableEntityAdapter.ReadItemAsync<T>(itemId, partitionKey, cancellationToken);
      }
      catch (Exception exception)
      {
        document.OperationException = exception;
        document.IsOperationSuccessful = false;
      }

      return document;
    }

    public async Task<(DocumentCollectionDto<T> DocumentCollection, string ContinuationToken)> GetItemQueryableAsync<T>(string continuationToken, CancellationToken cancellationToken, Expression<Func<T, bool>> predicate, QueryRequestOptions queryRequestOptions = null) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();
      DocumentCollectionDto<T> resultDocumentCollection = new DocumentCollectionDto<T>() { IsOperationSuccessful = true };
      string token = "";

      if (predicate == null)
      {
        throw new ArgumentNullException($"{nameof(GetItemQueryableAsync)} expects a valid value for {nameof(predicate)}");
      }

      try
      {
        if (string.IsNullOrWhiteSpace(continuationToken))
        {
          continuationToken = null;
        }

        var queryable = _databaseTableEntityAdapter.GetItemLinqQueryable<T>(continuationToken, queryRequestOptions)
                                                   .Where(predicate);
        FeedIterator<T> feedIterator = _cosmosLinqQueryAdapter.ToFeedIterator(queryable);

        FeedResponse<T> feedResponse = await feedIterator.ReadNextAsync(cancellationToken);
        token = feedResponse.ContinuationToken;

        foreach (var item in feedResponse)
        {
          resultDocumentCollection.Results.Add(item);
        }
      }
      catch (Exception exception)
      {
        resultDocumentCollection.OperationException = exception;
        resultDocumentCollection.IsOperationSuccessful = false;
      }

      return (resultDocumentCollection, token);
    }

    public async Task<PagedDocumentCollectionDto<T>> GetItemQueryableByPageAsync<T>(int pageNumber, QueryRequestOptions queryRequestOptions, CancellationToken cancellationToken, Expression<Func<T, bool>> predicate) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();
      PagedDocumentCollectionDto<T> resultPagedDocumentCollection = new PagedDocumentCollectionDto<T>() { IsOperationSuccessful = true };

      if (pageNumber <= default(int))
      {
        throw new InvalidOperationException($"{nameof(GetItemQueryableByPageAsync)} expects a valid value for {nameof(pageNumber)}");
      }
      if (queryRequestOptions == null || queryRequestOptions.MaxItemCount <= default(int))
      {
        throw new ArgumentNullException($"{nameof(GetItemQueryableByPageAsync)} expects a valid value for {nameof(queryRequestOptions)}");
      }
      if (predicate == null)
      {
        throw new ArgumentNullException($"{nameof(GetItemQueryableByPageAsync)} expects a valid value for {nameof(predicate)}");
      }

      try
      {
        resultPagedDocumentCollection.PageNumber = pageNumber;
        int skippedItems = queryRequestOptions.MaxItemCount.Value * (pageNumber - 1);
        FeedResponse<T> feedResponse = default;

        var queryable = _databaseTableEntityAdapter.GetItemLinqQueryable<T>(null, queryRequestOptions)
                                                   .Where(predicate)
                                                   .Skip(skippedItems)
                                                   .Take(queryRequestOptions.MaxItemCount.Value);
        FeedIterator<T> feedIterator = _cosmosLinqQueryAdapter.ToFeedIterator(queryable);

        while (feedIterator.HasMoreResults)
        {
          feedResponse = await feedIterator.ReadNextAsync(cancellationToken);
          if (feedResponse != null && feedResponse.Any())
          {
            foreach (var item in feedResponse)
            {
              resultPagedDocumentCollection.Results.Add(item);
            }
          }
        }
      }
      catch (Exception exception)
      {
        resultPagedDocumentCollection.OperationException = exception;
        resultPagedDocumentCollection.IsOperationSuccessful = false;
      }

      return resultPagedDocumentCollection;
    }

    public async Task<OperationStatus> UpdateItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();
      OperationStatus resultStatus = new OperationStatus();

      if (string.IsNullOrWhiteSpace(partitionKey))
      {
        throw new ArgumentNullException($"{nameof(UpdateItemAsync)} expects a valid value for {nameof(partitionKey)}");
      }

      try
      {
        HttpStatusCode statusCode = await _databaseTableEntityAdapter.UpsertItemAsync(item, partitionKey, cancellationToken);

        if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.NoContent)
        {
          resultStatus.IsOperationSuccessful = true;
        }
        resultStatus.OperationMessage = statusCode.ToString();
      }
      catch (Exception exception)
      {
        resultStatus.OperationException = exception;
      }

      return resultStatus;
    }
  }
}
