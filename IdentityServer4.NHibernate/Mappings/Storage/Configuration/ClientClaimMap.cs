namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ClientClaimMap : ClassMapping<ClientClaim>
    {
        public ClientClaimMap()
        {
            Id(p => p.ID, map =>
            {
                map.Generator(Generators.Native);
                map.Column("Id");
            });

            Property(p => p.Type, map => 
            {
                map.Length(250);
                map.NotNullable(true);
            });

            Property(p => p.Value, map => 
            {
                map.Length(250);
                map.NotNullable(true);
            });
        }
    }
}
