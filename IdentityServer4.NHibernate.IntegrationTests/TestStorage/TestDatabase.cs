using System;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
    /// <summary>
    /// A database used for testing.
    /// </summary>
    public abstract class TestDatabase
    {
        protected TestDatabase(string connectionString, string databaseName, global::NHibernate.Cfg.Configuration config)
        {
            ConnectionString = connectionString ?? throw new ArgumentException(nameof(connectionString));
            DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            DbConfig = config ?? throw new ArgumentNullException(nameof(config));
        }

        public ISessionFactory SessionFactory { get; protected set; }

        protected string ConnectionString { get; }

        protected string DatabaseName { get; }

        protected global::NHibernate.Cfg.Configuration DbConfig { get; }

        public abstract void CreateEmptyDatabase();

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
            CreateEmptyDatabase();
            SchemaMetadataUpdater.QuoteTableAndColumns(DbConfig, Dialect.GetDialect(DbConfig.Properties));
            SessionFactory = DbConfig.BuildSessionFactory();
        }

        public abstract void DropIfExists();
    }
}
