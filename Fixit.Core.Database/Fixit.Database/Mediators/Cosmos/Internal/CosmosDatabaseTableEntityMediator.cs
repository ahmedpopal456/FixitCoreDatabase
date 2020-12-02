using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Database.Adapters;
using Fixit.Database.DataContracts;
using Fixit.Database.DataContracts.Documents;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Fixit.Database.Mediators.Cosmos.Internal
{
  internal class CosmosDatabaseTableEntityMediator : IDatabaseTableEntityMediator
  {
    private IDatabaseTableEntityAdapter _databaseTableEntityAdapter;
    private const int _defaultPageSize = 20;

    public CosmosDatabaseTableEntityMediator(IDatabaseTableEntityAdapter databaseTableEntityAdapter)
    {
      _databaseTableEntityAdapter = databaseTableEntityAdapter ?? throw new ArgumentNullException($"{nameof(CosmosDatabaseTableEntityMediator)} expects a value for {nameof(databaseTableEntityAdapter)}... null argument was provided");
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
        item.id = Guid.NewGuid().ToString();
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

      try
      {
        if (string.IsNullOrWhiteSpace(continuationToken))
        {
          continuationToken = null;
        }

        FeedIterator<T> feedIterator;
        feedIterator = _databaseTableEntityAdapter.GetItemLinqQueryable<T>(continuationToken, queryRequestOptions)
                                          .Where(predicate)
                                          .ToFeedIterator();

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
      if (queryRequestOptions == null || !queryRequestOptions.MaxItemCount.HasValue)
      {
        throw new ArgumentNullException($"{nameof(GetItemQueryableByPageAsync)} expects a valid value for {nameof(queryRequestOptions)}");
      }

      try
      {
        resultPagedDocumentCollection.PageNumber = pageNumber;
        int skippedItems = queryRequestOptions.MaxItemCount.Value * (pageNumber - 1);
        FeedResponse<T> feedResponse = default;

        FeedIterator<T> feedIterator = _databaseTableEntityAdapter.GetItemLinqQueryable<T>(null, queryRequestOptions)
                                                            .Where(predicate)
                                                            .Skip(skippedItems)
                                                            .Take(queryRequestOptions.MaxItemCount.Value)
                                                            .ToFeedIterator();
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
