using System.Runtime.Serialization;

namespace Fixit.Storage.DataContracts.Documents
{
  [DataContract]
  public class DocumentDto<T> : OperationStatus
  {
    [DataMember]
    public T Document { get; set; }
  }
}
