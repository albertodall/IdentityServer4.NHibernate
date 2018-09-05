namespace IdentityServer4.NHibernate.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using Options;
    using global::NHibernate;


    public class DatabaseFixture : IDisposable
    {
        protected static readonly ConfigurationStoreOptions ConfigurationStoreOptions = new ConfigurationStoreOptions();
        protected static readonly OperationalStoreOptions OperationalStoreOptions = new OperationalStoreOptions();

        protected static List<ISessionFactory> SessionFactories = new List<ISessionFactory>()
        {
            TestDatabaseBuilder.SQLServer2012TestDatabase("ids_nh_test", ConfigurationStoreOptions, OperationalStoreOptions)
        };

        public void Dispose()
        {
            foreach (var sf in SessionFactories)
            {
                // Database is dropped after dispose of the Session Factory
                sf?.Dispose();
            }
        }
    }
}