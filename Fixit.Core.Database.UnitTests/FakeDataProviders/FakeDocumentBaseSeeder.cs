using System;
using System.Collections.Generic;

namespace Fixit.Core.Database.UnitTests.Adapters
{
  public class FakeDocumentBaseSeeder : IFakeSeederAdapter<DocumentBase>
  {
    public IList<DocumentBase> SeedFakeDtos()
    {
      var firstDocumentBase = new DocumentBase()
      {
        id = null,
        EntityId = null
      };
      var secondDocumentBase = new DocumentBase()
      {
        id = Guid.NewGuid().ToString(),
        EntityId = "123"
      };

      return new List<DocumentBase>
      {
        firstDocumentBase,
        secondDocumentBase
      };
    }
  }
}