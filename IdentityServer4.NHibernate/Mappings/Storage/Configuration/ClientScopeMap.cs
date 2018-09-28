namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ClientScopeMap : ClassMapping<ClientScope>
    {
        public ClientScopeMap()
        {
            Id(p => p.ID);

            Property(p => p.Scope, map => 
            {
                map.Length(200);
                map.NotNullable(true);
            });
        }
    }
}
