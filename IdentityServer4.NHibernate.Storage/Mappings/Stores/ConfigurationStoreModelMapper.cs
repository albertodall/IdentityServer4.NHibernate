using System;
using IdentityServer4.NHibernate.Entities;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.Options;
using NHibernate.Mapping.ByCode;

namespace IdentityServer4.NHibernate.Mappings.Stores
{
    internal class ConfigurationStoreModelMapper : ModelMapperBase
    {
        private readonly ConfigurationStoreOptions _options;

        public ConfigurationStoreModelMapper(ConfigurationStoreOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            BeforeMapClass += BeforeMapConfigurationStoreClass;
        }

        /// <summary>
        /// Sets table name and table's schema following the rule that the table name is the same as the type name.
        /// </summary>
        /// <remarks>
        /// Exceptions to the rule:
        ///     - "ApiResourceClaim" class has to be mapped to the "ApiClaims" table.
        ///     - "ClientScope" class has to be mapped to the "ClientScopes" table.
        /// </remarks>
        private void BeforeMapConfigurationStoreClass(IModelInspector modelInspector, Type type, IClassAttributesMapper classCustomizer)
        {
            TableDefinition tableDef = null;
            if (type == typeof(ApiResourceClaim))
            {
                tableDef = GetTableDefinition(nameof(_options.ApiClaim), _options);
            }
            else if (type == typeof(ClientScope))
            {
                tableDef = GetTableDefinition(nameof(_options.ClientScopes), _options);
            }
            else
            {
                tableDef = GetTableDefinition(type.Name, _options);
            }

            if (tableDef != null)
            {
                classCustomizer.MapToTable(tableDef, _options);
            }

            // Common mapping rule for IDs
            classCustomizer.Id(map =>
            {
                map.Column("Id");
                map.Generator(Generators.Native);
            });
        }
    }
}
