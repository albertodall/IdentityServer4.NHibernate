using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings.Stores.Configuration
{
    internal class ApiResourceSecretMap : ClassMapping<ApiResourceSecret>
    {
        public ApiResourceSecretMap()
        {
            Id(p => p.ID);

            Property(p => p.Description, map => map.Length(1000));

            Property(p => p.Value, map =>
            {
                map.Length(4000);
                map.NotNullable(true);
            });

            Property(p => p.Type, map =>
            {
                map.Length(250);
                map.NotNullable(true);
            });
        }
    }
}
