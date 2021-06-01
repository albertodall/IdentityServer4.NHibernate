using System;
using System.Diagnostics;
using System.IO;
using IdentityServer4.NHibernate.Database;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.Options;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
    /// <summary>
    /// Methods for creating test databases and related session factories.
    /// </summary>
    internal static class TestDatabaseBuilder
    {
        internal static SQLServerTestDatabase SQLServer2012TestDatabase(string connectionString, string databaseName, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            SQLServerTestDatabase testDb = null;
            try
            {
                var dbConfig = Databases.SqlServer2012()
                    .UsingConnectionString($"{connectionString};Initial Catalog={databaseName}")
                    .EnableSqlStatementsLogging()
                    .AddConfigurationStoreMappings(configurationStoreOptions)
                    .AddOperationalStoreMappings(operationalStoreOptions)
                    .SetProperty(Environment.Hbm2ddlAuto, "create-drop");

                testDb = new SQLServerTestDatabase(connectionString, databaseName, dbConfig);
                testDb.Drop();
                testDb.Create();
                new SchemaExport(dbConfig).Execute(false, true, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                testDb?.Drop();
                throw;
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
                testDb.Drop();
                testDb.Create();
                new SchemaExport(dbConfig).Execute(false, true, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                testDb?.Drop();
                throw;
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
                new SchemaExport(dbConfig).Execute(false, true, false, testDb.ActiveConnection, TextWriter.Null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            return testDb;
        }

        internal static PostgreSQLTestDatabase PostgreSQLTestDatabase(string connectionString, string databaseName, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            PostgreSQLTestDatabase testDb = null;
            try
            {
                var dbConfig = Databases.PostgreSQL()
                    .UsingConnectionString($"{connectionString};Database={databaseName}")
                    .EnableSqlStatementsLogging()
                    .AddConfigurationStoreMappings(configurationStoreOptions)
                    .AddOperationalStoreMappings(operationalStoreOptions)
                    .SetProperty(Environment.Hbm2ddlAuto, "create-drop");

                testDb = new PostgreSQLTestDatabase(connectionString, databaseName, dbConfig);
                testDb.Drop();
                testDb.Create();
                new SchemaExport(dbConfig).Execute(false, true, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                testDb?.Drop();
                throw;
            }

            return testDb;
        }

        internal static MySqlTestDatabase MySQLTestDatabase(string connectionString, string databaseName, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            MySqlTestDatabase testDb = null;
            try
            {
                var dbConfig = Databases.MySql()
                    .UsingConnectionString($"{connectionString};Database={databaseName}")
                    .EnableSqlStatementsLogging()
                    .AddConfigurationStoreMappings(configurationStoreOptions)
                    .AddOperationalStoreMappings(operationalStoreOptions)
                    .SetProperty(Environment.Hbm2ddlAuto, "create-drop");

                testDb = new MySqlTestDatabase(connectionString, databaseName, dbConfig);
                testDb.Drop();
                testDb.Create();
                new SchemaExport(dbConfig).Execute(false, true, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                testDb?.Drop();
                throw;
            }

            return testDb;
        }
    }
}
