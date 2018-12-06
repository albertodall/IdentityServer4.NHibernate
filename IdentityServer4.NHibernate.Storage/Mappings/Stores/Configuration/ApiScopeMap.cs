using IdentityServer4.NHibernate.Storage.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Storage.Mappings.Stores.Configuration
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

            Set(p => p.UserClaims, map => 
            {
                map.Key(fk =>
                {
                    fk.Column("ApiScopeId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ApiScopeClaims_ApiScope");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },   
                r => r.OneToMany(m => m.Class(typeof(ApiScopeClaim)))
            );
        }
    }
}
