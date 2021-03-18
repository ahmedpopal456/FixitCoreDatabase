using System.Net;

namespace Fixit.Core.Database.Helpers
{
  public static class DatabaseValidators
  {
    public static bool IsSuccessStatusCode(HttpStatusCode statusCode)
    {
      return ((int)statusCode >= 200) && ((int)statusCode <= 299);
    }
  }
}
