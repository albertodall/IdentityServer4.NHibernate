using System;
using NHibernate;
using NHibernate.Engine;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
    using System.Data.Common;

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
