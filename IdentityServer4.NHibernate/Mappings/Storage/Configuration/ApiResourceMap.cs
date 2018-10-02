using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
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

            Set(p => p.Secrets, map =>
            {
                map.Key(fk =>
                {
                    fk.Column("ApiResourceId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ApiSecrets_ApiResource");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ApiSecret)))
            );

            Set(p => p.Scopes, map =>
            {
                map.Key(fk => 
                {
                    fk.Column("ApiResourceId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ApiScopes_ApiResource");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ApiScope)))
            );

            Set(p => p.UserClaims, map =>
            {
                map.Key(fk =>
                {
                    fk.Column("ApiResourceId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ApiResourceClaims_ApiResource");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ApiResourceClaim)))
            );
        }
    }
}
