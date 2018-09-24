namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ApiScopeMap : ClassMapping<ApiScope>
    {
        public ApiScopeMap()
        {
            Id(p => p.ID, map =>
            {
                map.Generator(Generators.Native);
                map.Column("Id");
            });

            Property(p => p.Name, map => 
            {
                map.Length(200);
                map.NotNullable(true);
                map.UniqueKey("UK_ApiScope_Name");
            });

            Property(p => p.DisplayName, map => map.Length(200));
            Property(p => p.Description, map => map.Length(1000));
            Property(p => p.Emphasize);
            Property(p => p.ShowInDiscoveryDocument);

            Set<ApiScopeClaim>("_userClaims", map => 
            {
                map.Key(fk =>
                {
                    fk.Column("ApiScopeId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ApiScopeClaims_ApiScope");
                });
                map.Access(Accessor.Field);
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },   
                r => r.OneToMany(m => m.Class(typeof(ApiScopeClaim)))
            );
        }
    }
}
