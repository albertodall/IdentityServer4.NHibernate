using IdentityServer4.NHibernate.Storage.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Storage.Mappings.Stores.Configuration
{
    internal class ClientIdPRestrictionMap : ClassMapping<ClientIdPRestriction>
    {
        public ClientIdPRestrictionMap()
        {
            Id(p => p.ID);

            Property(p => p.Provider, map =>
            {
                map.Length(200);
                map.NotNullable(true);
            });
        }
    }
}
