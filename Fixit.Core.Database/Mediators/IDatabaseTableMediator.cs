using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Core.Database.Mediators
{
  public interface IDatabaseTableMediator
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="containerId"></param>
    /// <param name="partitionKeyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IDatabaseTableEntityMediator> CreateContainerAsync(string containerId, string partitionKeyPath, CancellationToken cancellationToken);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="containerId"></param>
    /// <returns></returns>
    IDatabaseTableEntityMediator GetContainer(string containerId);
  }
}
