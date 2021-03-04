using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fixit.Core.Database.Adapters;
using Fixit.Core.Database.Adapters.Cosmos;
using Fixit.Core.Database.Mediators;
using Fixit.Core.Database.Mediators.Cosmos.Internal;
using Moq;

namespace Fixit.Core.Database.UnitTests.Mediators.Cosmos
{
  [TestClass]
  public class CosmosDatabaseTableEntityMediatorTests : TestBase
  {
    private IDatabaseTableEntityMediator _cosmosDatabaseTableEntityMediator;
    private Mock<ICosmosLinqQueryAdapter> _cosmosLinqQueryAdapter;
    private Mock<FeedIterator<DocumentBase>> _feedIterator;
    private Mock<FeedResponse<DocumentBase>> _feedResponse;

    private IEnumerable<DocumentBase> _fakeDocumentBases;

    [TestInitialize]
    public void TestInitialize()
    {
      _cosmosDatabaseTableEntityAdapter = new Mock<IDatabaseTableEntityAdapter>();
      _cosmosLinqQueryAdapter = new Mock<ICosmosLinqQueryAdapter>();
      _feedIterator = new Mock<FeedIterator<DocumentBase>>();
      _feedResponse = new Mock<FeedResponse<DocumentBase>>();

      // Create fake data objects
      _fakeDocumentBases = fakeDtoSeederFactory.CreateSeederFactory<DocumentBase>(new DocumentBase());

      _cosmosDatabaseTableEntityMediator = new CosmosDatabaseTableEntityMediator(_cosmosDatabaseTableEntityAdapter.Object, _cosmosLinqQueryAdapter.Object);
    }

    [TestMethod]
    [DataRow("", DisplayName = "Null_PartitionKey")]
    public async Task CreateItemAsync_PartitionKeyNullOrWhiteSpace_ThrowsException(string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      // Act
      // Assert
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _cosmosDatabaseTableEntityMediator.CreateItemAsync(_fakeDocumentBases.First(), partitionKey, cancellationToken));
    }

    [TestMethod]
    [DataRow("123", DisplayName = "Any_PartitionKey")]
    public async Task CreateItemAsync_CreateItemException_ReturnsException(string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.CreateItemAsync(It.IsAny<DocumentBase>(), partitionKey, It.IsAny<CancellationToken>()))
                                       .Throws(new Exception());

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.CreateItemAsync(_fakeDocumentBases.First(), partitionKey, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult.OperationException);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
    }

    [TestMethod]
    [DataRow("123", DisplayName = "Any_PartitionKey")]
    public async Task CreateItemAsync_CreateItemSuccess_ReturnsSuccess(string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      var fakeDocumentBaseEmpty = _fakeDocumentBases.First();
      var fakeDocumentBase = _fakeDocumentBases.Last();

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.CreateItemAsync(It.IsAny<DocumentBase>(), partitionKey, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(fakeDocumentBase);

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.CreateItemAsync(fakeDocumentBaseEmpty, partitionKey, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult.Document);
      Assert.IsNull(actionResult.OperationException);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
    }

    [TestMethod]
    [DataRow(null, "123", DisplayName = "Null_ItemId")]
    [DataRow("123456", " ", DisplayName = "Whitespace_PartitionKey")]
    public async Task DeleteItemAsync_ItemIdOrPartitionKeyNullOrWhiteSpace_ThrowsException(string itemId, string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      // Act
      // Assert
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _cosmosDatabaseTableEntityMediator.DeleteItemAsync<DocumentBase>(itemId, partitionKey, cancellationToken));
    }

    [TestMethod]
    [DataRow("123456", "123", DisplayName = "Any_IdAndPartitionKey")]
    public async Task DeleteItemAsync_DeleteItemException_ReturnsException(string itemId, string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.DeleteItemAsync<DocumentBase>(itemId, partitionKey, It.IsAny<CancellationToken>()))
                                       .Throws(new Exception());

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.DeleteItemAsync<DocumentBase>(itemId, partitionKey, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult.OperationException);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
    }

    [TestMethod]
    [DataRow("123456", "123", DisplayName = "Any_IdAndPartitionKey")]
    public async Task DeleteItemAsync_DeleteItemFailure_ReturnsBadStatus(string itemId, string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.DeleteItemAsync<DocumentBase>(itemId, partitionKey, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(HttpStatusCode.BadRequest);

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.DeleteItemAsync<DocumentBase>(itemId, partitionKey, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult.OperationMessage);
      Assert.IsNull(actionResult.OperationException);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
    }

    [TestMethod]
    [DataRow("123456", "123", DisplayName = "Any_IdAndPartitionKey")]
    public async Task DeleteItemAsync_DeleteItemSuccess_ReturnsSuccess(string itemId, string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.DeleteItemAsync<DocumentBase>(itemId, partitionKey, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(HttpStatusCode.OK);

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.DeleteItemAsync<DocumentBase>(itemId, partitionKey, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult.OperationMessage);
      Assert.IsNull(actionResult.OperationException);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
    }

    [TestMethod]
    [DataRow(null, "123", DisplayName = "Null_ItemId")]
    [DataRow("123456", " ", DisplayName = "Null_PartitionKey")]
    public async Task GetItemAsync_ItemIdOrPartitionKeyNullOrWhiteSpace_ThrowsException(string itemId, string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      // Act
      // Assert
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _cosmosDatabaseTableEntityMediator.GetItemAsync<DocumentBase>(itemId, partitionKey, cancellationToken));
    }

    [TestMethod]
    [DataRow("123456", "123", DisplayName = "Any_IdAndPartitionKey")]
    public async Task GetItemAsync_GetItemException_ReturnsException(string itemId, string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.ReadItemAsync<DocumentBase>(itemId, partitionKey, It.IsAny<CancellationToken>()))
                                       .Throws(new Exception());

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.GetItemAsync<DocumentBase>(itemId, partitionKey, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult.OperationException);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
    }

    [TestMethod]
    [DataRow("123456", "123", DisplayName = "Any_IdAndPartitionKey")]
    public async Task GetItemAsync_GetItemSuccess_ReturnsSuccess(string itemId, string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      var fakeDocumentBase = _fakeDocumentBases.Last();

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.ReadItemAsync<DocumentBase>(itemId, partitionKey, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(fakeDocumentBase);

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.GetItemAsync<DocumentBase>(itemId, partitionKey, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult.Document);
      Assert.IsNull(actionResult.OperationException);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
    }

    [TestMethod]
    public async Task GetItemQueryableAsync_NullPredicate_ThrowsException()
    {
      // Arrange
      var continuationToken = " ";
      var cancellationToken = CancellationToken.None;

      // Act
      // Assert
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _cosmosDatabaseTableEntityMediator.GetItemQueryableAsync<DocumentBase>(continuationToken, cancellationToken, null));
    }

    [TestMethod]
    public async Task GetItemQueryableAsync_OperationException_ReturnsException()
    {
      // Arrange
      var continuationToken = " ";
      var cancellationToken = CancellationToken.None;

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.GetItemLinqQueryable<DocumentBase>(continuationToken, It.IsAny<QueryRequestOptions>()))
                                       .Throws(new Exception());

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.GetItemQueryableAsync<DocumentBase>(continuationToken, cancellationToken, i => i.EntityId == "123");

      // Assert
      Assert.IsNotNull(actionResult.DocumentCollection.OperationException);
      Assert.IsFalse(actionResult.DocumentCollection.IsOperationSuccessful);
    }

    [TestMethod]
    public async Task GetItemQueryableAsync_OperationSuccess_ReturnsSuccess()
    {
      // Arrange
      var continuationToken = " ";
      var cancellationToken = CancellationToken.None;
      List<DocumentBase> documents = new List<DocumentBase>() { _fakeDocumentBases.Last(), _fakeDocumentBases.Last() };
      var orderedQueryable = (from doc in documents select doc).AsQueryable().OrderBy( i => i.EntityId);

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.GetItemLinqQueryable<DocumentBase>(null, null))
                                       .Returns(orderedQueryable);
      _cosmosLinqQueryAdapter.Setup(cosmosLinqQuery => cosmosLinqQuery.ToFeedIterator(It.IsAny<IQueryable<DocumentBase>>()))
                             .Returns(_feedIterator.Object);
      _feedIterator.Setup(feedIterator => feedIterator.ReadNextAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(_feedResponse.Object);
      _feedResponse.Setup(feedResponse => feedResponse.GetEnumerator())
                   .Returns(documents.GetEnumerator());

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.GetItemQueryableAsync<DocumentBase>(continuationToken, cancellationToken, i => i.EntityId == "123");

      // Assert
      Assert.IsNotNull(actionResult.DocumentCollection.Results);
      Assert.IsNull(actionResult.DocumentCollection.OperationException);
      Assert.IsTrue(actionResult.DocumentCollection.IsOperationSuccessful);
    }

    [TestMethod]
    [DataRow(-1, DisplayName = "Invalid_PageNumber")]
    public async Task GetItemQueryableByPageAsync_InvalidPageNumber_ThrowsException(int pageNumber)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      // Act
      // Assert
      await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _cosmosDatabaseTableEntityMediator.GetItemQueryableByPageAsync(pageNumber, It.IsAny<QueryRequestOptions>(), cancellationToken, It.IsAny<Expression<Func<DocumentBase, bool>>>()));
    }

    [TestMethod]
    [DataRow(2, 0, DisplayName = "NullOrInvalid_PageSize")]
    public async Task GetItemQueryableByPageAsync_NullMaxItemCount_ThrowsException(int pageNumber, int pageSize)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      var queryRequestOptions = new QueryRequestOptions() { MaxItemCount = pageSize };

      // Act
      // Assert
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _cosmosDatabaseTableEntityMediator.GetItemQueryableByPageAsync(pageNumber, queryRequestOptions, cancellationToken, It.IsAny<Expression<Func<DocumentBase, bool>>>()));
    }

    [TestMethod]
    [DataRow(2, 5, DisplayName = "Any_PageNumberPageSize")]
    public async Task GetItemQueryableByPageAsync_NullPredicate_ThrowsException(int pageNumber, int pageSize)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      var queryRequestOptions = new QueryRequestOptions() { MaxItemCount = pageSize };

      // Act
      // Assert
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _cosmosDatabaseTableEntityMediator.GetItemQueryableByPageAsync<DocumentBase>(pageNumber, queryRequestOptions, cancellationToken, null));
    }


    [TestMethod]
    [DataRow(2, 5, DisplayName = "Any_PageNumberPageSize")]
    public async Task GetItemQueryableByPageAsync_OperationException_ReturnsException(int pageNumber, int pageSize)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      var queryRequestOptions = new QueryRequestOptions() { MaxItemCount = pageSize };

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.GetItemLinqQueryable<DocumentBase>(null, It.IsAny<QueryRequestOptions>()))
                                       .Throws(new Exception());

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.GetItemQueryableByPageAsync<DocumentBase>(pageNumber, queryRequestOptions, cancellationToken, i => i.EntityId == "123");

      // Assert
      Assert.IsNotNull(actionResult.OperationException);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
    }

    [TestMethod]
    [DataRow(2, 5, DisplayName = "Any_PageNumberPageSize")]
    public async Task GetItemQueryableByPageAsync_OperationSuccess_ReturnsSuccess(int pageNumber, int pageSize)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;
      var queryRequestOptions = new QueryRequestOptions() { MaxItemCount = pageSize };
      List<DocumentBase> documents = new List<DocumentBase>() { _fakeDocumentBases.Last(), _fakeDocumentBases.Last() };
      var orderedQueryable = (from doc in documents select doc).AsQueryable().OrderBy(i => i.EntityId);

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.GetItemLinqQueryable<DocumentBase>(null, It.IsAny<QueryRequestOptions>()))
                                       .Returns(orderedQueryable);
      _cosmosLinqQueryAdapter.Setup(cosmosLinqQuery => cosmosLinqQuery.ToFeedIterator(It.IsAny<IQueryable<DocumentBase>>()))
                             .Returns(_feedIterator.Object);
      _feedIterator.SetupSequence(feedIterator => feedIterator.HasMoreResults)
                   .Returns(true)
                   .Returns(false);
      _feedIterator.Setup(feedIterator => feedIterator.ReadNextAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(_feedResponse.Object);
      _feedResponse.Setup(feedResponse => feedResponse.GetEnumerator())
                   .Returns(documents.GetEnumerator());

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.GetItemQueryableByPageAsync<DocumentBase>(pageNumber, queryRequestOptions, cancellationToken, i => i.EntityId == "123");

      // Assert
      Assert.IsNotNull(actionResult.PageNumber);
      Assert.IsNull(actionResult.OperationException);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null_PartitionKey")]
    public async Task UpdateItemAsync_PartitionKeyNullOrWhiteSpace_ThrowsException(string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      // Act
      // Assert
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _cosmosDatabaseTableEntityMediator.UpdateItemAsync(_fakeDocumentBases.Last(), partitionKey, cancellationToken));
    }

    [TestMethod]
    [DataRow("123", DisplayName = "Any_PartitionKey")]
    public async Task UpdateItemAsync_UpdateItemException_ReturnsException(string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.UpsertItemAsync(It.IsAny<DocumentBase>(), partitionKey, It.IsAny<CancellationToken>()))
                                       .Throws(new Exception());

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.UpdateItemAsync(_fakeDocumentBases.Last(), partitionKey, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult.OperationException);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
    }

    [TestMethod]
    [DataRow("123", DisplayName = "Any_PartitionKey")]
    public async Task UpdateItemAsync_UpdateItemFailure_ReturnsBadStatus(string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.UpsertItemAsync(It.IsAny<DocumentBase>(), partitionKey, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(HttpStatusCode.BadRequest);

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.UpdateItemAsync(_fakeDocumentBases.Last(), partitionKey, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult.OperationMessage);
      Assert.IsNull(actionResult.OperationException);
      Assert.IsFalse(actionResult.IsOperationSuccessful);
    }

    [TestMethod]
    [DataRow("123", DisplayName = "Any_PartitionKey")]
    public async Task UpdateItemAsync_UpdateItemSuccess_ReturnsSuccess(string partitionKey)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      _cosmosDatabaseTableEntityAdapter.Setup(databaseTableEntityAdapter => databaseTableEntityAdapter.UpsertItemAsync(It.IsAny<DocumentBase>(), partitionKey, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(HttpStatusCode.OK);

      // Act
      var actionResult = await _cosmosDatabaseTableEntityMediator.UpdateItemAsync(_fakeDocumentBases.Last(), partitionKey, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult.OperationMessage);
      Assert.IsNull(actionResult.OperationException);
      Assert.IsTrue(actionResult.IsOperationSuccessful);
    }

    [TestCleanup]
    public void TestCleanup()
    {
      // Clean-up mock objects
      _cosmosDatabaseTableEntityAdapter.Reset();
      _cosmosLinqQueryAdapter.Reset();
      _feedIterator.Reset();
      _feedResponse.Reset();

      // Clean-up data objects
      _fakeDocumentBases = null;
    }
  }
}
