namespace IdentityServer4.NHibernate.IntegrationTests
{
    using System.Diagnostics;
    using Database;
    using Extensions;
    using Options;
    using global::NHibernate;
    using global::NHibernate.Tool.hbm2ddl;

    /// <summary>
    /// Methods for creating test databases and related session factories.
    /// </summary>
    internal static class TestDatabaseBuilder
    {
        public static SQLServerTestDatabase SQLServer2012TestDatabase(string serverName, string databaseName, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            var connString = $"Data Source={serverName}; Initial Catalog={databaseName}; Integrated Security=SSPI; Application Name=IdentityServer4.NHibernate.Test";

            var testDb = new SQLServerTestDatabase(serverName, databaseName);
            testDb.Create();

            ISessionFactory sessionFactory = null;
            try
            {
                var dbConfig = Databases.SqlServer2012()
                    .UsingConnectionString(connString)
                    .AddConfigurationStoreMappings(configurationStoreOptions)
                    .AddOperationalStoreMappings(operationalStoreOptions)
                    .SetProperty(global::NHibernate.Cfg.Environment.Hbm2ddlAuto, "create-drop");

                new SchemaExport(dbConfig).Create(false, true);
                sessionFactory = dbConfig.BuildSessionFactory();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            testDb.SetSessionFactory(sessionFactory);
            return testDb;
        }
    }
}
