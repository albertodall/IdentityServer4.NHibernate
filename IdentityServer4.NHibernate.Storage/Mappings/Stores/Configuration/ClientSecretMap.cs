using IdentityServer4.NHibernate.Storage.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Storage.Mappings.Stores.Configuration
{
    internal class ClientSecretMap : ClassMapping<ClientSecret>
    {
        public ClientSecretMap()
        {
            Id(p => p.ID);

            Property(p => p.Description, map => map.Length(2000));

            Property(p => p.Value, map =>
            {
                map.Length(2000);
                map.NotNullable(true);
            });

            Property(p => p.Expiration);
        }
    }
}
