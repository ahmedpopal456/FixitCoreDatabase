using System;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;


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
                throw new ArgumentNullException($"The account endpoint in the configuration file {nameof(configuration)} was not defined.");
            }
            if (string.IsNullOrWhiteSpace(authenticationKey))
            {
                throw new ArgumentNullException($"The connection string in the configuration file {nameof(configuration)} was not defined.");
            }

            _accountEndpoint = accountEndpoint;
            _authenticationKey = authenticationKey;
        }

        public void CreateCosmosClient()
        {
            throw new NotImplementedException();
        }
    }
}
