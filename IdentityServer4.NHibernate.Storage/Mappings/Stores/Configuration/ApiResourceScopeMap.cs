using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings.Stores.Configuration
{
    internal class ApiResourceScopeMap : ClassMapping<ApiResourceScope>
    {
        public ApiResourceScopeMap()
        {
            Id(p => p.ID);

            Property(p => p.Scope, map =>
            {
                map.NotNullable(true);
                map.Length(200);
            });
        }
    }
}
