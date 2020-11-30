using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Storage.Mediators
{
  public interface IDatabaseMediator
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IDatabaseTableMediator> CreateDatabaseAsync(string databaseId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseId"></param>
    /// <returns></returns>
    IDatabaseTableMediator GetDatabase(string databaseId);
  }
}
