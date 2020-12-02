using System;

namespace Fixit.Database.DataContracts
{
  // TODO: add Fixit error code definitions
  public class OperationStatus
  {
    public bool IsOperationSuccessful { get; set; }

    public string OperationMessage { get; set; }

  #nullable enable
    public Exception? OperationException { get; set; }
  #nullable disable
  }
}
