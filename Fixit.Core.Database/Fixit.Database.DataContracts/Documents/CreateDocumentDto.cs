using System.Runtime.Serialization;

namespace Fixit.Database.DataContracts.Documents
{
  [DataContract]
  public class CreateDocumentDto<T> : OperationStatus
  {
    [DataMember]
    public T Document { get; set; }
  }
}
