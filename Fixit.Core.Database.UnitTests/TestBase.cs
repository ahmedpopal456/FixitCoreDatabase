using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Fixit.Core.Database.UnitTests.Adapters;
using Fixit.Core.Database.Adapters;
using Fixit.Core.DataContracts.Seeders;

namespace Fixit.Core.Database.UnitTests
{
  public class TestBase
  {
    public IFakeSeederFactory fakeDtoSeederFactory;

    protected Mock<IDatabaseAdapter> _cosmosDatabaseAdapter;
    protected Mock<IDatabaseTableAdapter> _cosmosDatabaseTableAdapter;
    protected Mock<IDatabaseTableEntityAdapter> _cosmosDatabaseTableEntityAdapter;

    public TestBase()
    {
      fakeDtoSeederFactory = new FakeDtoSeederFactory();
    }

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext) { }

    [AssemblyCleanup]
    public static void AfterSuiteTests() { }
  }
}
