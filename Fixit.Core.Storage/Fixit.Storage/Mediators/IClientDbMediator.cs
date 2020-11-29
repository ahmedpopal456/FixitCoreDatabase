using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Storage.Mediators
{
  public interface IClientDbMediator
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IClientDbTableMediator> CreateDatabaseAsync(string databaseId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseId"></param>
    /// <returns></returns>
    IClientDbTableMediator GetDatabase(string databaseId);
  }
}
