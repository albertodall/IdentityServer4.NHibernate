using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings.Stores.Configuration
{
    internal class ApiResourcePropertyMap : ClassMapping<ApiResourceProperty>
    {
        public ApiResourcePropertyMap()
        {
            Id(p => p.ID);

            Property(x => x.Key, map => 
            {
                map.Column("[Key]");
                map.Length(250);
                map.NotNullable(true);
            });

            Property(x => x.Value, map => 
            {
                map.Length(2000);
                map.NotNullable(true);
            });
        }
    }
}
