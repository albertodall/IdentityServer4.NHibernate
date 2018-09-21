namespace IdentityServer4.NHibernate.Extensions
{
    using Mappings.Storage;
    using Mappings.Storage.Configuration;

    internal static class ConfigurationStoreModelMapperExtensions
    {
        public static ConfigurationStoreModelMapper AddClientContextMappings(this ConfigurationStoreModelMapper mapper)
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

        public static ConfigurationStoreModelMapper AddResourceContextMappings(this ConfigurationStoreModelMapper mapper)
        {
            mapper.AddMapping<IdentityResourceMap>();
            mapper.AddMapping<IdentityClaimMap>();

            return mapper;
        }
    }
}
