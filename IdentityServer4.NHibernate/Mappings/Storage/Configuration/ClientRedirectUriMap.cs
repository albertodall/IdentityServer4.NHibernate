namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ClientRedirectUriMap : ClassMapping<ClientRedirectUri>
    {
        public ClientRedirectUriMap()
        {
            Id(p => p.ID);

            Property(p => p.RedirectUri, map =>
            {
                map.Length(2000);
                map.NotNullable(true);
            });
        }
    }
}