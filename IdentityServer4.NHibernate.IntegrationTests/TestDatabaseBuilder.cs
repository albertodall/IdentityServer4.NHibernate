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
                .UsingConnectionString($"Data Source=(local); Initial Catalog={databaseName}; Integrated Security=SSPI; Application Name=IdentityServer4.NHibernate.Test")
                .AddConfigurationStoreMappings(configurationStoreOptions)
                .AddOperationalStoreMappings(operationalStoreOptions)
                .SetProperty(Environment.Hbm2ddlAuto, "create-drop");

            ISessionFactory sf = null;

            try
            {
                new SchemaExport(dbConfig).Create(false, true);
                sf = dbConfig.BuildSessionFactory();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return sf;
        }
    }
}
