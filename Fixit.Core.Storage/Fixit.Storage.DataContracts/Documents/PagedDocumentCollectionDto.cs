using System;
using System.Collections.Generic;
using System.Text;

namespace Fixit.Storage.DataContracts.Documents
{
  public class PagedDocumentCollectionDto<T> : DocumentCollectionDto<T>
  {
    public int PageNumber { get; set; }
  }
}
