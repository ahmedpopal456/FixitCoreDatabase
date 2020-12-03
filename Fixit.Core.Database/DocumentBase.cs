using System.Runtime.Serialization;

namespace Fixit.Core.Database
{
  [DataContract]
  public class DocumentBase
  {

    [DataMember]
    public string id { get; internal set; }

    [DataMember]
    public string EntityId { get; internal set; }
  }
}
