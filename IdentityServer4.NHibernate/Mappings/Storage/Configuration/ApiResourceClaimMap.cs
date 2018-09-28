using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    internal class ApiResourceClaimMap : ClassMapping<ApiResourceClaim>
    {
        public ApiResourceClaimMap()
        {
            Id(p => p.ID);

            Property(p => p.Type, map => 
            {
                map.Length(200);
                map.NotNullable(true);
            });
        }
    }
}
