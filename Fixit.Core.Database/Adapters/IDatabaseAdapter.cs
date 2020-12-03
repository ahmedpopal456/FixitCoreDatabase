using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Core.Database.Adapters
{
  public interface IDatabaseAdapter: IDisposable
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IDatabaseTableAdapter> CreateDatabaseIfNotExistsAsync(string databaseId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseId"></param>
    /// <returns></returns>
    IDatabaseTableAdapter GetDatabase(string databaseId);
  }
}
