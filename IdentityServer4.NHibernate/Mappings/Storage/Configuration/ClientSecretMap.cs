namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;
    using global::NHibernate.Type;

    internal class ClientSecretMap : ClassMapping<ClientSecret>
    {
        public ClientSecretMap()
        {
            Id(p => p.ID, map =>
            {
                map.Generator(Generators.Native);
                map.Column("Id");
            });

            Property(p => p.Description, map => map.Length(2000));

            Property(p => p.Value, map =>
            {
                map.Length(2000);
                map.NotNullable(true);
            });

            Property(p => p.Expiration, map => map.Type<DateTimeType>());
        }
    }
}
