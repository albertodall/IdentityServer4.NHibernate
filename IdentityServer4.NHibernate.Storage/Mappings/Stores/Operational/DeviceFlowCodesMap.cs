using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings.Stores.Operational
{
    internal class DeviceFlowCodesMap : ClassMapping<DeviceFlowCodes>
    {
        public DeviceFlowCodesMap()
        {
            Id(p => p.ID, map =>
            {
                map.Column("[UserCode]");
                map.Generator(Generators.Assigned);
                map.Length(200);
            });

            Property(p => p.DeviceCode, map =>
            {
                map.Length(200);
                map.UniqueKey("UK_DeviceCode");
                map.NotNullable(true);
            });

            Property(p => p.SubjectId, map => map.Length(200));

            Property(p => p.ClientId, map =>
            {
                map.Length(200);
                map.NotNullable(true);
            });

            Property(p => p.CreationTime, map => map.NotNullable(true));
            Property(p => p.Expiration, map => map.NotNullable(true));
            Property(p => p.Data, map => map.NotNullable(true));
        }
    }
}
