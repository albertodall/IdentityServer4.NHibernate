using System;
using System.Collections.Generic;
using IdentityServer4.NHibernate.IntegrationTests.TestStorage;

namespace IdentityServer4.NHibernate.IntegrationTests
{
    public class DatabaseFixture : IDisposable
    {
        public List<TestDatabase> TestDatabases;

        public void Dispose()
        {
            if (TestDatabases != null)
            {
                foreach (var db in TestDatabases)
                {
                    // Database objects are dropped after dispose of the Session Factory
                    db.SessionFactory?.Dispose();
                    // Drops the physical database
                    db.Drop();
                }
            }
        }
    }
}