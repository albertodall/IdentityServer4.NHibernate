using System;
using NHibernate;

namespace IdentityServer4.NHibernate.IntegrationTests
{
    using Microsoft.SqlServer.Management.Smo;

    /// <summary>
    /// A database used for testing.
    /// </summary>
    public abstract class TestDatabase
    {
        private ISessionFactory _sessionFactory;

        public TestDatabase(string databaseName)
        {
            DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        }

        public string DatabaseName { get; protected set; }

        public ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    throw new InvalidOperationException("SessionFactory still not set for this database.");
                }
                return _sessionFactory;
            }
            private set => _sessionFactory = value;
        }

        public void SetSessionFactory(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
        }

        public abstract void Create();

        public abstract void Drop();
    }

    /// <summary>
    /// A SQL Server database used for testing.
    /// </summary>
    internal class SQLServerTestDatabase : TestDatabase
    {
        public SQLServerTestDatabase(string serverName, string databaseName)
            : base(databaseName)
        {
            ServerName = serverName ?? throw new ArgumentNullException(nameof(serverName));
        }

        public override void Create()
        {
            var dbServer = new Server(ServerName);
            if (!dbServer.Databases.Contains(DatabaseName))
            {
                new Database(dbServer, DatabaseName).Create();
            }
        }

        public override void Drop()
        {
            var dbServer = new Server(ServerName);
            if (dbServer.Databases.Contains(DatabaseName))
            {
                // Kill all connections and then drop the database.
                dbServer.KillDatabase(DatabaseName);
            }
        }

        public string ServerName { get; }
    }
}
