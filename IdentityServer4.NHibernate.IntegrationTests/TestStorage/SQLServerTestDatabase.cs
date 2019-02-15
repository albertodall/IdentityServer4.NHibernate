using System;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
    using Microsoft.SqlServer.Management.Smo;

    /// <summary>
    /// A SQL Server database used for testing.
    /// </summary>
    internal class SQLServerTestDatabase : TestDatabase
    {
        private readonly string _serverName;
        private readonly string _databaseName;

        public SQLServerTestDatabase(string serverName, string databaseName, global::NHibernate.Cfg.Configuration config)
            : base(config)
        {
            _serverName = serverName ?? throw new ArgumentNullException(nameof(serverName));
            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        }

        public override void Create()
        {
            var dbServer = new Server(_serverName);
            if (!dbServer.Databases.Contains(_databaseName))
            {
                new Database(dbServer, _databaseName).Create();
            }
            base.Create();
        }

        public override void Drop()
        {
            var dbServer = new Server(_serverName);
            if (dbServer.Databases.Contains(_databaseName))
            {
                // Kill all connections and then drop the database.
                dbServer.KillDatabase(_databaseName);
            }
        }
    }
}
