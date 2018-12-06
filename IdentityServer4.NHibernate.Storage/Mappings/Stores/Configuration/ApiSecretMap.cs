using IdentityServer4.NHibernate.Storage.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Storage.Mappings.Stores.Configuration
{
    internal class ApiSecretMap : ClassMapping<ApiSecret>
    {
        public ApiSecretMap()
        {
            Id(p => p.ID);

            Property(p => p.Description, map => map.Length(1000));
            Property(p => p.Value, map => map.Length(2000));
            Property(p => p.Type, map => map.Length(250));
        }
    }
}
