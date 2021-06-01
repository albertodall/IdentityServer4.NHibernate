using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
    using Microsoft.SqlServer.Management.Smo;

    /// <summary>
    /// A SQL Server database used for testing.
    /// </summary>
    internal class SQLServerTestDatabase : TestDatabase
    {
        public SQLServerTestDatabase(string connectionString, string databaseName, global::NHibernate.Cfg.Configuration config)
            : base(connectionString, databaseName, config)
        {  }

        public override void CreateEmptyDatabase()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var dbServer = new Server(new ServerConnection(conn));
                if (!dbServer.Databases.Contains(DatabaseName))
                {
                    new Database(dbServer, DatabaseName).Create();
                }
            }
        }

        public override void Drop()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var dbServer = new Server(new ServerConnection(conn));
                if (dbServer.Databases.Contains(DatabaseName))
                {
                    // Kill all connections and then drop the database.
                    dbServer.KillDatabase(DatabaseName);
                }
            }
        }
    }
}
