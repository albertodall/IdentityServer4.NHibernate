using IdentityServer4.NHibernate.Mappings.Stores;
using IdentityServer4.NHibernate.Options;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("IdentityServer4.NHibernate.IntegrationTests")]

namespace IdentityServer4.NHibernate.Extensions
{
    using global::NHibernate.Cfg;

    /// <summary>
    /// Extension methods for configuring NHibernate.
    /// </summary>
    public static class NHibernateConfigurationExtensions
    {
        /// <summary>
        /// Sets the connection string to access the database.
        /// </summary>
        /// <param name="configuration">The NHibernate configuration.</param>
        /// <param name="connectionString">The connection string.</param>
        public static Configuration UsingConnectionString(this Configuration configuration, string connectionString)
        {
            configuration.SetProperty(Environment.ConnectionString, connectionString);
            return configuration;
        }

        /// <summary>
        /// Sets the connection string name to access the database (defined in configuration file).
        /// </summary>
        /// <param name="configuration">The NHibernate configuration.</param>
        /// <param name="connectionStringName">The connection string name.</param>
        public static Configuration UsingConnectionStringName(this Configuration configuration, string connectionStringName)
        {
            configuration.SetProperty(Environment.ConnectionStringName, connectionStringName);
            return configuration;
        }

        /// <summary>
        /// Enables logging of generated SQL statements to console.
        /// </summary>
        /// <param name="configuration">The NHibernate configuration.</param>
        public static Configuration EnableSqlStatementsLogging(this Configuration configuration)
        {
            configuration.SetProperty(Environment.ShowSql, bool.TrueString);
            configuration.SetProperty(Environment.FormatSql, bool.TrueString);
            configuration.SetProperty(Environment.UseSqlComments, bool.TrueString);
            return configuration;
        }

        /// <summary>
        /// Loads all mappings related to IdentityServer configuration into the NHibernate configuration.
        /// </summary>
        /// <param name="configuration">The NHibernate configuration.</param>
        /// <param name="options">The configuration store options.</param>
        public static Configuration AddConfigurationStoreMappings(this Configuration configuration, ConfigurationStoreOptions options)
        {
            var mapper = new ConfigurationStoreModelMapper(options);
            mapper.AddClientContextMappings();
            mapper.AddResourceContextMappings();
            configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
            return configuration;
        }

        /// <summary>
        /// Loads all mappings related to IdentityServer operational store into the NHibernate configuration.
        /// </summary>
        /// <param name="configuration">The NHibernate configuration.</param>
        /// <param name="options">The operational store options.</param>
        public static Configuration AddOperationalStoreMappings(this Configuration configuration, OperationalStoreOptions options)
        {
            var mapper = new OperationalStoreModelMapper(options);
            mapper.AddPersistedGrantContextMappings();
            configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
            return configuration;
        }
    }
}
