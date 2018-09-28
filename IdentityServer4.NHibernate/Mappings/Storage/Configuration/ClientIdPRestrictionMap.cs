namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ClientIdPRestrictionMap : ClassMapping<ClientIdPRestriction>
    {
        public ClientIdPRestrictionMap()
        {
            Id(p => p.ID);

            Property(p => p.Provider, map =>
            {
                map.Length(200);
                map.NotNullable(true);
            });
        }
    }
}
