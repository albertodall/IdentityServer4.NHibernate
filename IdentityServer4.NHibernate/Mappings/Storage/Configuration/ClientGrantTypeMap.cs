namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ClientGrantTypeMap : ClassMapping<ClientGrantType>
    {
        public ClientGrantTypeMap()
        {
            Id(p => p.ID, map => 
            {
                map.Generator(Generators.Native);
                map.Column("Id");
            });

            Property(p => p.GrantType, map => 
            {
                map.Length(250);
                map.NotNullable(true);
            });
        }
    }
}
