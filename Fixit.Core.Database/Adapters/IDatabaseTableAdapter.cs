using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Core.Database.Adapters
{
  public interface IDatabaseTableAdapter
  {
    /// <summary>
    ///  
    /// </summary>
    /// <param name="containerId"></param>
    /// <param name="partitionKeyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IDatabaseTableEntityAdapter> CreateContainerIfNotExistsAsync(string containerId, string partitionKeyPath, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="containerId"></param>
    /// <returns></returns>
    IDatabaseTableEntityAdapter GetContainer(string containerId);
  }
}
