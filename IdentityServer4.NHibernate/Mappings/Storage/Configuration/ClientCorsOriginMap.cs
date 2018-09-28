namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

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
