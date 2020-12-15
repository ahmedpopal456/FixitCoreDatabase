using Fixit.Core.DataContracts;

namespace Fixit.Core.Database.UnitTests.Adapters
{
  public class FakeDtoSeederFactory : IFakeSeederFactory
  {
    public IFakeSeederAdapter<T> CreateFakeSeeder<T>() where T : class
    {
      string type = typeof(T).Name;

      switch (type)
      {
        case nameof(DocumentBase):
          return (IFakeSeederAdapter<T>) new FakeDocumentBaseSeeder();
        default:
          return null;
      }
    }
  }
}
