using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Storage.Mediators
{
  public interface IClientDbTableMediator
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="containerId"></param>
    /// <param name="partitionKeyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IClientDbTableEntityMediator> CreateContainerAsync(string containerId, string partitionKeyPath, CancellationToken cancellationToken);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="containerId"></param>
    /// <returns></returns>
    IClientDbTableEntityMediator GetContainer(string containerId);
  }
}
