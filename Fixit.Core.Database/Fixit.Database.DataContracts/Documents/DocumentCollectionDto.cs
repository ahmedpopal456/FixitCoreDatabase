using System.Collections.Generic;

namespace Fixit.Database.DataContracts.Documents
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
