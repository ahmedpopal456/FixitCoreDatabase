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
using Fixit.Core.Database.Helpers;
using Fixit.Core.DataContracts.Decorators.Exceptions;

[assembly: InternalsVisibleTo("Fixit.Core.Database.UnitTests")]
namespace Fixit.Core.Database.Mediators.Cosmos.Internal
{
  internal class CosmosDatabaseTableEntityMediator : IDatabaseTableEntityMediator
  {
    private IDatabaseTableEntityAdapter _databaseTableEntityAdapter;
    private ICosmosLinqQueryAdapter _cosmosLinqQueryAdapter;
    private OperationStatusExceptionDecorator _decorator;

    public CosmosDatabaseTableEntityMediator(IDatabaseTableEntityAdapter databaseTableEntityAdapter, ICosmosLinqQueryAdapter cosmosLinqQueryAdapter = null)
    {
      _databaseTableEntityAdapter = databaseTableEntityAdapter ?? throw new ArgumentNullException($"{nameof(CosmosDatabaseTableEntityMediator)} expects a value for {nameof(databaseTableEntityAdapter)}... null argument was provided");
      _cosmosLinqQueryAdapter = cosmosLinqQueryAdapter ?? new CosmosLinqQueryAdapter();
      _decorator = new OperationStatusExceptionDecorator();
    }

    public async Task<CreateDocumentDto<T>> CreateItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (string.IsNullOrWhiteSpace(partitionKey))
      {
        throw new ArgumentNullException($"{nameof(CreateItemAsync)} expects a valid value for {nameof(partitionKey)}");
      }

      CreateDocumentDto<T> resultCreateDocument = new CreateDocumentDto<T>() { IsOperationSuccessful = true };

      resultCreateDocument = (CreateDocumentDto<T>)await _decorator.ExecuteOperationAsync(resultCreateDocument, async () => {
        item.id ??= Guid.NewGuid().ToString();
        item.EntityId = partitionKey;

        resultCreateDocument.Document = await _databaseTableEntityAdapter.CreateItemAsync(item, partitionKey, cancellationToken);
      });
      return resultCreateDocument;
    }

    public async Task<OperationStatus> DeleteItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (string.IsNullOrWhiteSpace(itemId))
      {
        throw new ArgumentNullException($"{nameof(DeleteItemAsync)} expects a valid value for {nameof(itemId)}");
      }
      if (string.IsNullOrWhiteSpace(partitionKey))
      {
        throw new ArgumentNullException($"{nameof(DeleteItemAsync)} expects a valid value for {nameof(partitionKey)}");
      }

      OperationStatus resultStatus = new OperationStatus();

      resultStatus = await _decorator.ExecuteOperationAsync(resultStatus, async () => {
        HttpStatusCode statusCode = await _databaseTableEntityAdapter.DeleteItemAsync<T>(itemId, partitionKey, cancellationToken);

        resultStatus.IsOperationSuccessful = DatabaseValidators.IsSuccessStatusCode(statusCode);
        resultStatus.OperationMessage = statusCode.ToString();
      });
      return resultStatus;
    }

    public async Task<DocumentDto<T>> GetItemAsync<T>(string itemId, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (string.IsNullOrWhiteSpace(itemId))
      {
        throw new ArgumentNullException($"{nameof(GetItemAsync)} expects a valid value for {nameof(itemId)}");
      }
      if (string.IsNullOrWhiteSpace(partitionKey))
      {
        throw new ArgumentNullException($"{nameof(GetItemAsync)} expects a valid value for {nameof(partitionKey)}");
      }

      DocumentDto<T> document = new DocumentDto<T>() { IsOperationSuccessful = true };

      document = (DocumentDto<T>)await _decorator.ExecuteOperationAsync(document, async () => {
        document.Document = await _databaseTableEntityAdapter.ReadItemAsync<T>(itemId, partitionKey, cancellationToken);
      });
      return document;
    }

    public async Task<(DocumentCollectionDto<T> DocumentCollection, string ContinuationToken)> GetItemQueryableAsync<T>(string continuationToken, CancellationToken cancellationToken, Expression<Func<T, bool>> predicate, QueryRequestOptions queryRequestOptions = null) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (predicate == null)
      {
        throw new ArgumentNullException($"{nameof(GetItemQueryableAsync)} expects a valid value for {nameof(predicate)}");
      }

      DocumentCollectionDto<T> resultDocumentCollection = new DocumentCollectionDto<T>() { IsOperationSuccessful = true };
      string token = "";

      resultDocumentCollection = (DocumentCollectionDto<T>)await _decorator.ExecuteOperationAsync(resultDocumentCollection, async () => {
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
      });
      return (resultDocumentCollection, token);
    }

    public async Task<PagedDocumentCollectionDto<T>> GetItemQueryableByPageAsync<T>(int pageNumber, QueryRequestOptions queryRequestOptions, CancellationToken cancellationToken, Expression<Func<T, bool>> predicate) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();
      
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

      PagedDocumentCollectionDto<T> resultPagedDocumentCollection = new PagedDocumentCollectionDto<T>() { IsOperationSuccessful = true };

      resultPagedDocumentCollection = (PagedDocumentCollectionDto<T>)await _decorator.ExecuteOperationAsync(resultPagedDocumentCollection, async () => {
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
      });
      return resultPagedDocumentCollection;
    }

    public async Task<OperationStatus> UpsertItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken) where T : DocumentBase
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (string.IsNullOrWhiteSpace(partitionKey))
      {
        throw new ArgumentNullException($"{nameof(UpsertItemAsync)} expects a valid value for {nameof(partitionKey)}");
      }

      OperationStatus resultStatus = new OperationStatus();

      resultStatus = await _decorator.ExecuteOperationAsync(resultStatus, async () => {
        item.id ??= Guid.NewGuid().ToString();
        item.EntityId ??= partitionKey;

        HttpStatusCode statusCode = await _databaseTableEntityAdapter.UpsertItemAsync(item, partitionKey, cancellationToken);

        resultStatus.IsOperationSuccessful = DatabaseValidators.IsSuccessStatusCode(statusCode);
        resultStatus.OperationMessage = statusCode.ToString();
      });
      return resultStatus;
    }
  }
}
