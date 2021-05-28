using System.Diagnostics;
using IdentityServer4.NHibernate.Database;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.Options;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
    /// <summary>
    /// Methods for creating test databases and related session factories.
    /// </summary>
    internal static class TestDatabaseBuilder
    {
        internal static SQLServerTestDatabase SQLServer2012TestDatabase(string serverName, string databaseName, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            var connString = $"Data Source={serverName}; Initial Catalog={databaseName}; Integrated Security=SSPI; Application Name=IdentityServer4.NHibernate.IntegrationTests";

            SQLServerTestDatabase testDb = null;
            try
            {
                var dbConfig = Databases.SqlServer2012()
                    .UsingConnectionString(connString)
                    .EnableSqlStatementsLogging()
                    .AddConfigurationStoreMappings(configurationStoreOptions)
                    .AddOperationalStoreMappings(operationalStoreOptions)
                    .SetProperty(Environment.Hbm2ddlAuto, "create-drop");

                testDb = new SQLServerTestDatabase(serverName, databaseName, dbConfig);
                testDb.Create();
                new SchemaExport(dbConfig).Execute(false, true, false);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                testDb?.Drop();
            }

            return testDb;
        }

        internal static SQLiteTestDatabase SQLiteTestDatabase(string fileName, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            var connString = $"Data Source={fileName};Version=3;Pooling=True;Synchronous=Full;";

            SQLiteTestDatabase testDb = null;
            try
            {
                var dbConfig = Databases.SQLite()
                    .UsingConnectionString(connString)
                    .EnableSqlStatementsLogging()
                    .AddConfigurationStoreMappings(configurationStoreOptions)
                    .AddOperationalStoreMappings(operationalStoreOptions)
                    .SetProperty(Environment.Hbm2ddlAuto, "create-drop");

                testDb = new SQLiteTestDatabase(fileName, dbConfig);
                testDb.Create();
                new SchemaExport(dbConfig).Execute(false, true, false);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                testDb?.Drop();
            }

            return testDb;
        }

        internal static SQLiteInMemoryTestDatabase SQLiteInMemoryTestDatabase(ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            SQLiteInMemoryTestDatabase testDb = null;
            try
            {
                var dbConfig = Databases.SQLiteInMemory()
                    .EnableSqlStatementsLogging()
                    .AddConfigurationStoreMappings(configurationStoreOptions)
                    .AddOperationalStoreMappings(operationalStoreOptions);

                testDb = new SQLiteInMemoryTestDatabase(dbConfig);
                testDb.Create();
                new SchemaExport(dbConfig).Execute(false, true, false, testDb.ActiveConnection, null);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return testDb;
        }

        internal static PostgreSQL83TestDatabase Postgres83TestDatabase(string serverName, string databaseName, string userName, string password, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            var connString = $"Server={serverName};Port=5432;User Id={userName};Password={password}";

            PostgreSQL83TestDatabase testDb = null;
            try
            {
                var dbConfig = Databases.PostgreSQL83()
                    .UsingConnectionString(connString)
                    .EnableSqlStatementsLogging()
                    .AddConfigurationStoreMappings(configurationStoreOptions)
                    .AddOperationalStoreMappings(operationalStoreOptions);

                testDb = new PostgreSQL83TestDatabase(connString, databaseName, dbConfig);
                testDb.Create();
                new SchemaExport(dbConfig).Execute(false, true, false);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                testDb?.Drop();
            }

            return testDb;
        }
    }
}
