using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.Database.Adapters;
using Fixit.Core.Database.DataContracts;
using Fixit.Core.Database.DataContracts.Documents;
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
