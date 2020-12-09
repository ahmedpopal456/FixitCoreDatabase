using System;
using System.Collections.Generic;
using System.Text;

namespace Fixit.Core.Database.UnitTests.Adapters
{
  public interface IFakeSeederAdapter<T> where T : class
  {
    IList<T> SeedFakeDtos();
  }
}
