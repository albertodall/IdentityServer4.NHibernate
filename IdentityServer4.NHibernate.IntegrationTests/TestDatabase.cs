using System;
using NHibernate;
using NHibernate.Engine;

namespace IdentityServer4.NHibernate.IntegrationTests
{
    using Microsoft.SqlServer.Management.Smo;
    using System.Data.Common;
    using System.Data.SQLite;

    /// <summary>
    /// A database used for testing.
    /// </summary>
    public abstract class TestDatabase
    {
        public TestDatabase(global::NHibernate.Cfg.Configuration config)
        {
            DBConfig = config ?? throw new ArgumentNullException(nameof(config));
        }

        public ISessionFactory SessionFactory { get; protected set; }

        protected global::NHibernate.Cfg.Configuration DBConfig { get; private set; }

        public virtual ISession OpenSession()
        {
            if (SessionFactory == null)
            {
                throw new InvalidOperationException("Session factory still not created.");
            }
            return SessionFactory.OpenSession();
        }

        public virtual IStatelessSession OpenStatelessSession()
        {
            if (SessionFactory == null)
            {
                throw new InvalidOperationException("Session factory still not created.");
            }
            return SessionFactory.OpenStatelessSession();
        }

        public virtual void Create()
        {
            SessionFactory = DBConfig.BuildSessionFactory();
        }

        public virtual void Drop() { }
    }

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

    /// <summary>
    /// A file-based SQLite database used for testing.
    /// </summary>
    internal class SQLiteTestDatabase : TestDatabase
    {
        private readonly string _databaseFileName;

        public SQLiteTestDatabase(string databaseFileName, global::NHibernate.Cfg.Configuration config)
            : base(config)
        {
            _databaseFileName = databaseFileName ?? throw new ArgumentNullException(nameof(databaseFileName));
        }

        public override void Create()
        {
            SQLiteConnection.CreateFile(_databaseFileName);
            base.Create();
        }
    }

    /// <summary>
    /// An in-memory SQLite database used for testing
    /// </summary>
    /// <remarks>
    /// By default SQLite in-memory databases are "per-connection", so different NH sessions use different databases.
    /// To share the in-memory database between sessions, the SchemaExport action that creates the schema and the Session that will
    /// act on the database must share the same connection. That's why this configuration gets the active connection thru the NHibernate's connection provider.
    /// </remarks>
    internal class SQLiteInMemoryTestDatabase : TestDatabase
    {
        public SQLiteInMemoryTestDatabase(global::NHibernate.Cfg.Configuration config)
            : base(config)
        { }

        public override void Create()
        {
            base.Create();
            ActiveConnection = ((SessionFactory as ISessionFactoryImplementor).ConnectionProvider).GetConnection();
        }

        public override ISession OpenSession()
        {
            if (SessionFactory == null)
            {
                throw new InvalidOperationException("Session factory still not created.");
            }
            return SessionFactory.WithOptions().Connection(ActiveConnection).OpenSession();
        }

        public override IStatelessSession OpenStatelessSession()
        {
            if (SessionFactory == null)
            {
                throw new InvalidOperationException("Session factory still not created.");
            }
            return SessionFactory.OpenStatelessSession(ActiveConnection);
        }

        public DbConnection ActiveConnection { get; private set; }
    }
}
