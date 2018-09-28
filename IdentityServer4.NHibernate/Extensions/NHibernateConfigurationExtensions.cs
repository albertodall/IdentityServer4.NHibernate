using IdentityServer4.NHibernate.Mappings.Storage;
using IdentityServer4.NHibernate.Mappings.Storage.Operational;
using IdentityServer4.NHibernate.Options;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("IdentityServer4.NHibernate.IntegrationTests")]

namespace IdentityServer4.NHibernate.Extensions
{
    using global::NHibernate.Cfg;
    
    public static class NHibernateConfigurationExtensions
    {
        /// <summary>
        /// Sets the connection string to access the database.
        /// </summary>
        /// <param name="configuration">The NHibernate configuration.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public static Configuration UsingConnectionStringName(this Configuration configuration, string connectionStringName)
        {
            configuration.SetProperty(Environment.ConnectionStringName, connectionStringName);
            return configuration;
        }

        /// <summary>
        /// Loads all mappings related to IdentityServer configuration into the NHibernate configuration.
        /// </summary>
        /// <param name="configuration">The NHibernate configuration.</param>
        /// <param name="options">The configuration store options.</param>
        internal static Configuration AddConfigurationStoreMappings(this Configuration configuration, ConfigurationStoreOptions options)
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
        internal static Configuration AddOperationalStoreMappings(this Configuration configuration, OperationalStoreOptions options)
        {
            var mapper = new OperationalStoreModelMapper(options);
            mapper.AddMapping<PersistedGrantMap>();
            configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
            return configuration;
        }
    }
}
