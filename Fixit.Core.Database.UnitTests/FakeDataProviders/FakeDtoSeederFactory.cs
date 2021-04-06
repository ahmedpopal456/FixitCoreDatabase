using Fixit.Core.DataContracts.Seeders;
using System.Collections.Generic;

namespace Fixit.Core.Database.UnitTests.Adapters
{
  public class FakeDtoSeederFactory : IFakeSeederFactory
  {
		public IList<T> CreateSeederFactory<T>(IFakeSeederAdapter<T> fakeSeederAdapter) where T : class
		{
			return fakeSeederAdapter.SeedFakeDtos();
		}
	}
}
