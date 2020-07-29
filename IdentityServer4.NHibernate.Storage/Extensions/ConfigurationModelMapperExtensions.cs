using IdentityServer4.NHibernate.Mappings.Stores;
using IdentityServer4.NHibernate.Mappings.Stores.Configuration;
using IdentityServer4.NHibernate.Mappings.Stores.Operational;

namespace IdentityServer4.NHibernate.Extensions
{
    internal static class ConfigurationStoreModelMapperExtensions
    {
        internal static ConfigurationStoreModelMapper AddClientContextMappings(this ConfigurationStoreModelMapper mapper)
        {
            mapper.AddMapping<ClientMap>();
            mapper.AddMapping<ClientGrantTypeMap>();
            mapper.AddMapping<ClientSecretMap>();
            mapper.AddMapping<ClientRedirectUriMap>();
            mapper.AddMapping<ClientPostLogoutRedirectUriMap>();
            mapper.AddMapping<ClientScopeMap>();
            mapper.AddMapping<ClientIdPRestrictionMap>();
            mapper.AddMapping<ClientClaimMap>();
            mapper.AddMapping<ClientCorsOriginMap>();
            mapper.AddMapping<ClientPropertyMap>();

            return mapper;
        }

        internal static ConfigurationStoreModelMapper AddResourceContextMappings(this ConfigurationStoreModelMapper mapper)
        {
            mapper.AddMapping<IdentityResourceMap>();
            mapper.AddMapping<IdentityClaimMap>();
            mapper.AddMapping<IdentityResourcePropertyMap>();
            mapper.AddMapping<ApiResourceMap>();
            mapper.AddMapping<ApiResourcePropertyMap>();
            mapper.AddMapping<ApiResourceSecretMap>();
            mapper.AddMapping<ApiResourceClaimMap>();
            mapper.AddMapping<ApiResourceScopeMap>();
            mapper.AddMapping<ApiScopeMap>();
            mapper.AddMapping<ApiScopeClaimMap>();

            return mapper;
        }

        internal static OperationalStoreModelMapper AddPersistedGrantContextMappings(this OperationalStoreModelMapper mapper)
        {
            mapper.AddMapping<PersistedGrantMap>();
            mapper.AddMapping<DeviceFlowCodesMap>();

            return mapper;
        }
    }
}
