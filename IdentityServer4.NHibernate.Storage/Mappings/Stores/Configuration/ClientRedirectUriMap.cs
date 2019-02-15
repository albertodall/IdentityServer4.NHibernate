using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings.Stores.Configuration
{
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