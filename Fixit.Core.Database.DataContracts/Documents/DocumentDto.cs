﻿using System.Runtime.Serialization;
using Fixit.Core.DataContracts;

namespace Fixit.Core.Database.DataContracts.Documents
{
  [DataContract]
  public class DocumentDto<T> : OperationStatus
  {
    [DataMember]
    public T Document { get; set; }
  }
}
