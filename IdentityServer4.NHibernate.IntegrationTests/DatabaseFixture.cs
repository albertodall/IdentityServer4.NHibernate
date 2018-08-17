namespace IdentityServer4.NHibernate.IntegrationTests
{
    using System;
    using Database;
    using Extensions;
    using Options;
    using global::NHibernate.Tool.hbm2ddl;
    using Xunit;

    public class DatabaseFixture
    {
        private static readonly ConfigurationStoreOptions StoreOptions = new ConfigurationStoreOptions();

        [Fact]
        public void Should_Create_Database()
        {
            var dbConfig = Databases.SqlServer2012()
                .UsingConnectionString("Data Source=localhost; Initial Catalog=IdentityServer_NH_Test; Integrated Security=SSPI")
                .AddConfigurationStoreMappings(StoreOptions)
                .SetProperty(global::NHibernate.Cfg.Environment.Hbm2ddlAuto, "create-drop");

            var schemaExporter = new SchemaExport(dbConfig);
            schemaExporter.Create(true, true);

            var sessionFactory = dbConfig.BuildSessionFactory();

            Assert.True(true);

            sessionFactory.Dispose();
        }
    }
}
