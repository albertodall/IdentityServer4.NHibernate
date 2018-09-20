namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ClientPostLogoutRedirectUriMap : ClassMapping<ClientPostLogoutRedirectUri>
    {
        public ClientPostLogoutRedirectUriMap()
        {
            Id(p => p.ID, map =>
            {
                map.Generator(Generators.Native);
                map.Column("Id");
            });

            Property(p => p.PostLogoutRedirectUri, map => 
            {
                map.Length(2000);
                map.NotNullable(true);
            });
        }
    }
}
