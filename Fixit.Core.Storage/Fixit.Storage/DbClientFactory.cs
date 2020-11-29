using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Fixit.Storage.Adapters.Internal;
using Fixit.Storage.Mediators;
using Fixit.Storage.Mediators.Internal;


namespace Fixit.Storage
{
  public class DbClientFactory
  {
    private readonly string _accountEndpoint;
    private readonly string _authenticationKey;

    public DbClientFactory(IConfiguration configuration)
    {
      string accountEndpoint = configuration["FIXIT-SA-EP"];
      string authenticationKey = configuration["FIXIT-SA-CS"];

      if (string.IsNullOrWhiteSpace(accountEndpoint))
      {
        throw new ArgumentNullException($"{nameof(DbClientFactory)} expects a valid value for {nameof(accountEndpoint)} within the configuration file");
      }
      if (string.IsNullOrWhiteSpace(authenticationKey))
      {
        throw new ArgumentNullException($"{nameof(DbClientFactory)} expects a valid value for {nameof(authenticationKey)} within the configuration file");
      }

      _accountEndpoint = accountEndpoint;
      _authenticationKey = authenticationKey;
    }

    public IClientDbMediator CreateCosmosClient()
    {
      CosmosClient cosmosClient = new CosmosClient(_accountEndpoint, _authenticationKey);
      CosmosDbAdapter cosmosDbAdapter = new CosmosDbAdapter(cosmosClient);
      return new CosmosDbMediator(cosmosDbAdapter) ;
    }
  }
}
