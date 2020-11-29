﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Storage.Adapters
{
  public interface IClientDbAdapter: IDisposable
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IClientDbTableAdapter> CreateDatabaseIfNotExistsAsync(string databaseId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseId"></param>
    /// <returns></returns>
    IClientDbTableAdapter GetDatabase(string databaseId);
  }
}
