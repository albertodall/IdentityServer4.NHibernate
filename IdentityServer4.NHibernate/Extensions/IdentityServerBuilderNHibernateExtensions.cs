namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using IdentityServer4.NHibernate.Options;
    using IdentityServer4.NHibernate.Services;
    using IdentityServer4.NHibernate.Stores;
    using IdentityServer4.NHibernate.TokenCleanup;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;
    using Microsoft.Extensions.Hosting;
    using NHibernate.Cfg;

    public static class IdentityServerBuilderNHibernateExtensions
    {
        /// <summary>
        /// Configures NHibernate implementation of IClientStore, IResourceStore, and ICorsPolicyService with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="databaseConfiguration">The configuration to access the underlying database.</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder,
            Configuration databaseConfiguration,
            Action<ConfigurationStoreOptions> storeOptionsAction)
        {
            var options = new ConfigurationStoreOptions();
            builder.Services.AddSingleton(options);
            storeOptionsAction?.Invoke(options);

            databaseConfiguration.SetProperty(NHibernate.Cfg.Environment.DefaultSchema, options.DefaultSchema);

            builder.Services.AddSingleton(databaseConfiguration.BuildSessionFactory());

            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            return builder;
        }

        /// <summary>
        /// Configures NHibernate implementation of IPersistedGrantStore with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="databaseConfiguration">The configuration to access the underlying database.</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
            Configuration databaseConfiguration,
            Action<OperationalStoreOptions> storeOptionsAction)
        {
            builder.Services.AddSingleton<TokenCleanup>();
            builder.Services.AddSingleton<IHostedService, TokenCleanupHost>();

            var storeOptions = new OperationalStoreOptions();
            builder.Services.AddSingleton(storeOptions);
            storeOptionsAction?.Invoke(storeOptions);

            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            return builder;
        }
    }
}