using Fixit.Core.Database.DataContracts.Documents;
using Microsoft.Azure.Cosmos.Table;

namespace Fixit.Core.Database.DataContracts.Documents
{
  public class SegmentedDocumentCollectionDto<T> : DocumentCollectionDto<T>
  {
    public TableContinuationToken TableContinuationToken { get; set; }
  }
}
