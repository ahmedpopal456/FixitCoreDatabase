using System;
using System.Collections.Generic;
using System.Text;

namespace Fixit.Core.Database.UnitTests.Adapters
{
  public interface IFakeSeederFactory
  {
    IFakeSeederAdapter<T> CreateFakeSeeder<T>() where T : class;
  }
}
