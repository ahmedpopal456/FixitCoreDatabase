using System.Linq;
using Microsoft.Azure.Cosmos;

namespace Fixit.Core.Database.Adapters.Cosmos
{
  public interface ICosmosLinqQueryAdapter
  {
    FeedIterator<T> ToFeedIterator<T>(IQueryable<T> query);
  }
}
