using System.Runtime.Serialization;

namespace Fixit.Core.Database.DataContracts.Documents
{
  [DataContract]
  public class PagedDocumentCollectionDto<T> : DocumentCollectionDto<T>
  {
    [DataMember]
    public int PageNumber { get; set; }
  }
}
