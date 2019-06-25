using System;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace MyTemplate.Core.Repository
{
    public sealed class ConnectionFactory : IConnectionFactory
    {
        private string _connectionName;
        private IConfiguration _configuration;
        private string _constr;

        public ConnectionFactory(IConfiguration configuration, string connectionName = "DefaultConnection")
        {
            _configuration = configuration;
            _connectionName = connectionName;
            _constr = _configuration[$"connectionStrings:database:{_connectionName}"];
        }

        public ConnectionFactory(string connectionString)
        {
            _constr = connectionString;
        }

        public DbConnection GetConnection()
        {
            var connection = new System.Data.SqlClient.SqlConnection(_constr);

            return connection;
        }
    }
}
