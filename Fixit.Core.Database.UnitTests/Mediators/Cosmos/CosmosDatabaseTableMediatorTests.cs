using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.Database.Mediators;
using Fixit.Core.Database.Mediators.Cosmos.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fixit.Core.Database.UnitTests.Mediators.Cosmos
{
  [TestClass]
  public class CosmosDatabaseTableMediatorTests : TestBase
  {
    private IDatabaseTableMediator _cosmosDatabaseTableMediator;

    [TestInitialize]
    public void TestInitialize()
    {
      _cosmosDatabaseTableMediator = new CosmosDatabaseTableMediator(_cosmosDatabaseTableAdapter.Object);
    }

    [TestMethod]
    [DataRow(null, "/EntityId", DisplayName = "Null_ContainerId")]
    [DataRow("container", "", DisplayName = "Null_ContainerId")]
    public async Task CreateDatabaseAsync_ContainerIdOrPartitionKeyPathNullOrWhiteSpace_ThrowsArgumentNullException(string containerId, string partitionKeyPath)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      // Act
      // Assert
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _cosmosDatabaseTableMediator.CreateContainerAsync(containerId, partitionKeyPath, cancellationToken));
    }

    [TestMethod]
    [DataRow("container", "/EntityId", DisplayName = "Any_ContainerIdAndPartitionKeyPath")]
    public async Task CreateDatabaseAsync_CreateDatabaseSuccess_ReturnsSuccess(string containerId, string partitionKeyPath)
    {
      // Arrange
      var cancellationToken = CancellationToken.None;

      _cosmosDatabaseTableAdapter.Setup(databaseTableAdapter => databaseTableAdapter.CreateContainerIfNotExistsAsync(containerId, partitionKeyPath, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(_cosmosDatabaseTableEntityAdapter.Object);

      // Act
      var actionResult = await _cosmosDatabaseTableMediator.CreateContainerAsync(containerId, partitionKeyPath, cancellationToken);

      // Assert
      Assert.IsNotNull(actionResult);
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null_ContainerId")]
    public void GetDatabase_DatabaseIdNullOrWhiteSpace_ThrowsArgumentNullException(string containerId)
    {
      // Arrange
      // Act
      // Assert
      Assert.ThrowsException<ArgumentNullException>(() => _cosmosDatabaseTableMediator.GetContainer(containerId));
    }

    [TestMethod]
    [DataRow("container", DisplayName = "Any_ContainerId")]
    public void GetDatabase_GetDatabaseSuccess_ReturnsSuccess(string containerId)
    {
      // Arrange
      _cosmosDatabaseTableAdapter.Setup(databaseTableAdapter => databaseTableAdapter.GetContainer(containerId))
                                 .Returns(_cosmosDatabaseTableEntityAdapter.Object);

      // Act
      var actionResult = _cosmosDatabaseTableMediator.GetContainer(containerId);

      // Assert
      Assert.IsNotNull(actionResult);
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }
  }
}
