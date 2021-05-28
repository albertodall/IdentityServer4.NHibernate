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
        protected TestDatabase(global::NHibernate.Cfg.Configuration config)
        {
            DbConfig = config ?? throw new ArgumentNullException(nameof(config));
        }

        public ISessionFactory SessionFactory { get; private set; }

        private global::NHibernate.Cfg.Configuration DbConfig { get; }

        public virtual ISession OpenSession()
        {
            if (SessionFactory == null)
            {
                throw new InvalidOperationException("Session factory not yet created.");
            }
            return SessionFactory.OpenSession();
        }

        public virtual IStatelessSession OpenStatelessSession()
        {
            if (SessionFactory == null)
            {
                throw new InvalidOperationException("Session factory not yet created.");
            }
            return SessionFactory.OpenStatelessSession();
        }

        public virtual void Create()
        {
            SchemaMetadataUpdater.QuoteTableAndColumns(DbConfig, Dialect.GetDialect(DbConfig.Properties));
            SessionFactory = DbConfig.BuildSessionFactory();
        }

        public virtual void Drop() { }
    }
}
