﻿namespace Fixit.Core.Database.DataContracts.Documents
{
  public class PagedDocumentCollectionDto<T> : DocumentCollectionDto<T>
  {
    public int PageNumber { get; set; }
  }
}