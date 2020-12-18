using System.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Fixit.Core.Database.Adapters.Cosmos
{
  public class CosmosLinqQueryAdapter : ICosmosLinqQueryAdapter
  {
    public FeedIterator<T> ToFeedIterator<T>(IQueryable<T> query)
    {
      return query.ToFeedIterator();
    }
  }
}
