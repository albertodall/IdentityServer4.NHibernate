using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    internal class ClientPropertyMap : ClassMapping<ClientProperty>
    {
        public ClientPropertyMap()
        {
            Id(p => p.ID);

            Property(p => p.Key, map => 
            {
                map.Column("[Key]");
                map.Length(250);
                map.NotNullable(true);
            });

            Property(p => p.Value, map => 
            {
                map.Length(2000);
                map.NotNullable(true);
            });
        }
    }
}
