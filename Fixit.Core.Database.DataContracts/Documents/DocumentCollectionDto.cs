using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.DataContracts;

namespace Fixit.Core.Database.DataContracts.Documents
{
  [DataContract]
  public class DocumentCollectionDto<T> : OperationStatus
  {
    private IList<T> _results;

    [DataMember]
    public IList<T> Results
    {
      get => _results ??= new List<T>();
      set => _results = value;
    }
  }
}
