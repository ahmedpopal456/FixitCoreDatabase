using System.Runtime.Serialization;

namespace Fixit.Database.DataContracts.Documents
{
  [DataContract]
  public class DocumentDto<T> : OperationStatus
  {
    [DataMember]
    public T Document { get; set; }
  }
}
