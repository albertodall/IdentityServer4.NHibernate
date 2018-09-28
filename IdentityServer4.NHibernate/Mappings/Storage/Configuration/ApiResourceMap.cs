namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ApiResourceMap : ClassMapping<ApiResource>
    {
        public ApiResourceMap()
        {
            Id(p => p.ID);

            Property(p => p.Name, map => 
            {
                map.Length(200);
                map.NotNullable(true);
                map.UniqueKey("UK_ApiResource_Name");
            });

            Property(p => p.DisplayName, map => map.Length(200));
            Property(p => p.Description, map => map.Length(1000));

            Set<ApiSecret>("_secrets", map =>
            {
                map.Key(fk =>
                {
                    fk.Column("ApiResourceId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ApiSecrets_ApiResource");
                });
                map.Access(Accessor.Field);
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ApiSecret)))
            );

            Set<ApiScope>("_scopes", map =>
            {
                map.Key(fk => fk.Column("ApiResourceId"));
                map.Access(Accessor.Field);
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ApiScope)))
            );

            Set<ApiResourceClaim>("_userClaims", map =>
            {
                map.Key(fk =>
                {
                    fk.Column("ApiResourceId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ApiResourceClaims_ApiResource");
                });
                map.Access(Accessor.Field);
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ApiResourceClaim)))
            );
        }
    }
}
