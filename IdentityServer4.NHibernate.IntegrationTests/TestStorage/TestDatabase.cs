using System;
using NHibernate;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
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
}
