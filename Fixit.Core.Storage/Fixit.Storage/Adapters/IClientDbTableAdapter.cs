﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Storage.Adapters
{
  public interface IClientDbTableAdapter
  {
    /// <summary>
    ///  
    /// </summary>
    /// <param name="containerId"></param>
    /// <param name="partitionKeyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IClientDbTableEntityAdapter> CreateContainerIfNotExistsAsync(string containerId, string partitionKeyPath, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="containerId"></param>
    /// <returns></returns>
    IClientDbTableEntityAdapter GetContainer(string containerId);
  }
}
