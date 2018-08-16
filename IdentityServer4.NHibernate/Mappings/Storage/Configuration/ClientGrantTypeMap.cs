namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ClientGrantTypeMap : ClassMapping<ClientGrantType>
    {
        public ClientGrantTypeMap()
        {
            Id(p => p.ID, map => map.Generator(Generators.Native));

            Property(p => p.GrantType, map => {
                map.Length(250);
                map.NotNullable(true);
            });

            ManyToOne(p => p.Client);
        }
    }
}
