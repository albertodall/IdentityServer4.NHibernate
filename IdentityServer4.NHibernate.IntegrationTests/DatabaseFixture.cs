namespace IdentityServer4.NHibernate.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using global::NHibernate;

    public class DatabaseFixture : IDisposable
    {
        public List<ISessionFactory> SessionFactories;

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