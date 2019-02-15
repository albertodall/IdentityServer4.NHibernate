using System;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.Mappings.Stores;
using IdentityServer4.NHibernate.Options;

namespace IdentityServer4.NHibernate.Database.Schema
{
    using global::NHibernate.Cfg;
    using global::NHibernate.Tool.hbm2ddl;

#pragma warning disable 1591

    public static class ScriptCreator
    {
        /// <summary>
        /// Scripts the database objects creation for storing IdentityServer data.
        /// </summary>
        /// <param name="scriptFileName">Output file name</param>
        /// <param name="configuration">NHibernate Configuration object that represents the database configuration to script.</param>
        /// <param name="configurationStoreOptions">Options for configuration store.</param>
        /// <param name="operationalStoreOptions">Options for operational store.</param>
        /// <remarks>
        /// Configuration store options and operational store options are needed to detect the schema for each database object.
        /// </remarks>
        public static void CreateSchemaScriptForDatabase(
            string scriptFileName,
            Configuration configuration, 
            ConfigurationStoreOptions configurationStoreOptions, 
            OperationalStoreOptions operationalStoreOptions)
        {
            if (configurationStoreOptions == null)
            {
                throw new ArgumentNullException(nameof(configurationStoreOptions));
            }

            if (operationalStoreOptions == null)
            {
                throw new ArgumentNullException(nameof(operationalStoreOptions));
            }

            var configurationStoreMapper = new ConfigurationStoreModelMapper(configurationStoreOptions);
            var operationalStoreMapper = new OperationalStoreModelMapper(operationalStoreOptions);

            configuration.AddConfigurationStoreMappings(configurationStoreOptions);
            configuration.AddOperationalStoreMappings(operationalStoreOptions);

            new SchemaExport(configuration)
                .SetOutputFile(scriptFileName)
                .Execute(true, false, false);
        }
    }
}
