using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Fixit.Storage.Adapters.Internal;
using Fixit.Storage.Mediators;
using Fixit.Storage.Mediators.Internal;


namespace Fixit.Storage
{
  public class DatabaseFactory
  {
    private readonly string _accountEndpoint;
    private readonly string _authenticationKey;

    public DatabaseFactory(IConfiguration configuration)
    {
      string accountEndpoint = configuration["FIXIT-SA-EP"];
      string authenticationKey = configuration["FIXIT-SA-CS"];

      if (string.IsNullOrWhiteSpace(accountEndpoint))
      {
        throw new ArgumentNullException($"{nameof(DatabaseFactory)} expects a valid value for {nameof(accountEndpoint)} within the configuration file");
      }
      if (string.IsNullOrWhiteSpace(authenticationKey))
      {
        throw new ArgumentNullException($"{nameof(DatabaseFactory)} expects a valid value for {nameof(authenticationKey)} within the configuration file");
      }

      _accountEndpoint = accountEndpoint;
      _authenticationKey = authenticationKey;
    }

    public IDatabaseMediator CreateCosmosClient()
    {
      CosmosClient cosmosClient = new CosmosClient(_accountEndpoint, _authenticationKey);
      CosmosDatabaseAdapter cosmosDbAdapter = new CosmosDatabaseAdapter(cosmosClient);
      return new CosmosDatabaseMediator(cosmosDbAdapter) ;
    }
  }
}
