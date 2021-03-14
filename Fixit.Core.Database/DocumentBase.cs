using Fixit.Core.DataContracts.Seeders;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[assembly: InternalsVisibleTo("Fixit.Core.Database.UnitTests")]
namespace Fixit.Core.Database
{
  [DataContract]
  public class DocumentBase: IFakeSeederAdapter<DocumentBase>
  {
    [DataMember]
    public string id { get; internal set; }

    [DataMember]
    public string EntityId { get; internal set; }

    public IList<DocumentBase> SeedFakeDtos()
    {
      var firstDocumentBase = new DocumentBase()
      {
        id = null,
        EntityId = null
      };
      var secondDocumentBase = new DocumentBase()
      {
        id = Guid.NewGuid().ToString(),
        EntityId = "123"
      };

      return new List<DocumentBase>
      {
        firstDocumentBase,
        secondDocumentBase
      };
        }
  }
}
