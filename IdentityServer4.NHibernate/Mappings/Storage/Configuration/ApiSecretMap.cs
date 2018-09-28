namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

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
