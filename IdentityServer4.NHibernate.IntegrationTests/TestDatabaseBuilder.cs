namespace IdentityServer4.NHibernate.IntegrationTests
{
    using Database;
    using Extensions;
    using Options;
    using global::NHibernate;
    using global::NHibernate.Cfg;
    using global::NHibernate.Tool.hbm2ddl;

    /// <summary>
    /// Methods for creating test databases and related session factories.
    /// </summary>
    internal static class TestDatabaseBuilder
    {
        public static ISessionFactory SQLServer2012TestDatabase(string databaseName, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            var dbConfig = Databases.SqlServer2012()
                .UsingConnectionString($"Data Source=localhost; Initial Catalog={databaseName}; Integrated Security=SSPI")
                .AddConfigurationStoreMappings(configurationStoreOptions)
                .AddOperationalStoreMappings(operationalStoreOptions)
                .SetProperty(Environment.Hbm2ddlAuto, "create-drop");

            var schemaExporter = new SchemaExport(dbConfig);
            schemaExporter.Create(true, true);

            return dbConfig.BuildSessionFactory();
        }
    }
}
