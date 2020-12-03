﻿using System.Runtime.Serialization;

namespace Fixit.Core.Database.DataContracts.Documents
{
  [DataContract]
  public class CreateDocumentDto<T> : OperationStatus
  {
    [DataMember]
    public T Document { get; set; }
  }
}
