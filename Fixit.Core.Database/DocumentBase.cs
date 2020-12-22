using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[assembly: InternalsVisibleTo("Fixit.Core.Database.UnitTests")]
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
