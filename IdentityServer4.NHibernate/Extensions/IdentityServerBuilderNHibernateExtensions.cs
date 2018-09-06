namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using IdentityServer4.NHibernate.Extensions;
    using IdentityServer4.NHibernate.Options;
    using IdentityServer4.NHibernate.Services;
    using IdentityServer4.NHibernate.Stores;
    using IdentityServer4.NHibernate.TokenCleanup;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;
    using Hosting;
    using NHibernate;
    using NHibernate.Cfg;

    public static class IdentityServerBuilderNHibernateExtensions
    {
        /// <summary>
        /// Configures NHibernate-based database support for IdentityServer.
        /// - Adds NHibernate implementation of IClientStore, IResourceStore, and ICorsPolicyService (configuration store)
        /// - Adds NHibernate implementation of IPersistedGrantStore and TokenCleanup (operational store).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="databaseConfiguration">The NHibernate configuration to access the underlying database.</param>
        /// <param name="configurationStoreOptionsAction">The configurations store options action.</param>
        /// <param name="operationalStoreOptionsAction">The operational store options action.</param>
        public static IIdentityServerBuilder AddDatabaseSupport(
            this IIdentityServerBuilder builder,
            Configuration databaseConfiguration,
            Action<ConfigurationStoreOptions> configurationStoreOptionsAction,
            Action<OperationalStoreOptions> operationalStoreOptionsAction)
        {
            var configStoreOptions = new ConfigurationStoreOptions();
            builder.Services.AddSingleton(configStoreOptions);
            configurationStoreOptionsAction?.Invoke(configStoreOptions);

            var operationalStoreOptions = new OperationalStoreOptions();
            builder.Services.AddSingleton(operationalStoreOptions);
            operationalStoreOptionsAction?.Invoke(operationalStoreOptions);

            // Adds NHibernate mappings
            databaseConfiguration.AddConfigurationStoreMappings(configStoreOptions);
            databaseConfiguration.AddOperationalStoreMappings(operationalStoreOptions);

            // Adds NHibernate objects to the DI system
            builder.Services.AddSingleton(databaseConfiguration.BuildSessionFactory());
            builder.Services.AddScoped(provider => 
            {
                var factory = provider.GetService<ISessionFactory>();
                return factory.OpenSession();
            });
            builder.Services.AddScoped(provider => 
            {
                var factory = provider.GetService<ISessionFactory>();
                return factory.OpenStatelessSession();
            });

            // Adds configuration store components
            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            // Adds operational store components.
            builder.Services.AddSingleton<TokenCleanup>();
            builder.Services.AddSingleton<IHostedService, TokenCleanupHost>();
            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            return builder;
        }

        /// <summary>
        /// Configures NHibernate-based database support for IdentityServer.
        /// - Adds NHibernate implementation of IClientStore, IResourceStore, and ICorsPolicyService (configuration store)
        /// - Adds NHibernate implementation of IPersistedGrantStore and TokenCleanup (operational store).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="databaseConfigurationFunction">Configuration function that configures NHibernate to access the underlying database.</param>
        /// <param name="configurationStoreOptionsAction">The configurations store options action.</param>
        /// <param name="operationalStoreOptionsAction">The operational store options action.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddDatabaseSupport(
            this IIdentityServerBuilder builder,
            Func<Configuration> databaseConfigurationFunction,
            Action<ConfigurationStoreOptions> configurationStoreOptionsAction,
            Action<OperationalStoreOptions> operationalStoreOptionsAction)
        {
            return builder.AddDatabaseSupport(
                databaseConfigurationFunction(),
                configurationStoreOptionsAction,
                operationalStoreOptionsAction);
        }
    }
}