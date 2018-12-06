using IdentityServer4.NHibernate.Storage.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Storage.Mappings.Stores.Configuration
{
    internal class ClientCorsOriginMap : ClassMapping<ClientCorsOrigin>
    {
        public ClientCorsOriginMap()
        {
            Id(p => p.ID);

            Property(p => p.Origin, map =>
            {
                map.Length(150);
                map.NotNullable(true);
            });
        }
    }
}
