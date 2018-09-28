using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    internal class ApiScopeMap : ClassMapping<ApiScope>
    {
        public ApiScopeMap()
        {
            Id(p => p.ID);

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
                map.Fetch(CollectionFetchMode.Join);
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },   
                r => r.OneToMany(m => m.Class(typeof(ApiScopeClaim)))
            );

            ManyToOne(p => p.ApiResource, map => 
            {
                map.Column("ApiResourceId");
                map.ForeignKey("FK_ApiScopes_ApiResource");
                map.NotNullable(true);
                map.Cascade(Cascade.Persist);
            });
        }
    }
}
