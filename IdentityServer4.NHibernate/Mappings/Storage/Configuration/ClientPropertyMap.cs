namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ClientPropertyMap : ClassMapping<ClientProperty>
    {
        public ClientPropertyMap()
        {
            Id(p => p.ID, map =>
            {
                map.Generator(Generators.Native);
                map.Column("Id");
            });

            Property(p => p.Key, map => 
            {
                map.Column("[Key]");
                map.Length(250);
                map.NotNullable(true);
            });

            Property(p => p.Value, map => 
            {
                map.Length(2000);
                map.NotNullable(true);
            });
        }
    }
}
