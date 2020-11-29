using System.Collections.Generic;

namespace Fixit.Storage.DataContracts.Documents
{
  public class DocumentCollectionDto<T> : OperationStatus
  {
    private IList<T> _results;
    public IList<T> Results
    {
      get => _results ??= new List<T>();
      set => _results = value;
    }
  }
}
