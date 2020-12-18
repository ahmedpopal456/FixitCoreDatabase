using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.Database.Adapters;
using Fixit.Core.Database.Mediators;
using Fixit.Core.Database.Mediators.Cosmos.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fixit.Core.Database.UnitTests.Mediators.Cosmos
{
  [TestClass]
  public class CosmosDatabaseMediatorTests : TestBase
  {
    private IDatabaseMediator _cosmosDatabaseMediator;

    [TestInitialize]
    public void TestInitialize()
    {
      _cosmosDatabaseAdapter = new Mock<IDatabaseAdapter>();
      _cosmosDatabaseTableAdapter = new Mock<IDatabaseTableAdapter>();

      _cosmosDatabaseMediator = new CosmosDatabaseMediator(_cosmosDatabaseAdapter.Object);
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null_DatabaseId")]
    public async Task CreateDatabaseAsync_DatabaseIdNullOrWhiteSpace_ThrowsArgumentNullException(string databaseId)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      // Act
      // Assert
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _cosmosDatabaseMediator.CreateDatabaseAsync(databaseId, cancellationToken));
    }

    [TestMethod]
    [DataRow("db", DisplayName = "Any_DatabaseId")]
    public async Task CreateDatabaseAsync_CreateDatabaseSuccess_ReturnsSuccess(string databaseId)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      _cosmosDatabaseAdapter.Setup(databaseAdapter => databaseAdapter.CreateDatabaseIfNotExistsAsync(databaseId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(_cosmosDatabaseTableAdapter.Object);

      // Act
      var actionResult = await _cosmosDatabaseMediator.CreateDatabaseAsync(databaseId, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult);
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null_DatabaseId")]
    public void GetDatabase_DatabaseIdNullOrWhiteSpace_ThrowsArgumentNullException(string databaseId)
    {
      // Arrange
      // Act
      // Assert
      Assert.ThrowsException<ArgumentNullException>(() => _cosmosDatabaseMediator.GetDatabase(databaseId));
    }

    [TestMethod]
    [DataRow("db", DisplayName = "Any_DatabaseId")]
    public void GetDatabase_GetDatabaseSuccess_ReturnsSuccess(string databaseId)
    {
      // Arrange
      _cosmosDatabaseAdapter.Setup(databaseAdapter => databaseAdapter.GetDatabase(databaseId))
                            .Returns(_cosmosDatabaseTableAdapter.Object);

      // Act
      var actionResult = _cosmosDatabaseMediator.GetDatabase(databaseId);

      // Assert
      Assert.IsNotNull(actionResult);
    }

    [TestCleanup]
    public void TestCleanup()
    {
      // Clean-up mock objects
      _cosmosDatabaseAdapter.Reset();
      _cosmosDatabaseTableAdapter.Reset();
    }
  }
}
