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
    }
}
