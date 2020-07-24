using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings.Stores.Configuration
{
    internal class IdentityResourceMap : ClassMapping<IdentityResource>
    {
        public IdentityResourceMap()
        {
            Id(p => p.ID);

            Property(p => p.Name, map => 
            {
                map.Length(200);
                map.NotNullable(true);
                map.UniqueKey("UK_IdentityResource_Name");
            });

            Property(p => p.DisplayName, map => map.Length(200));
            Property(p => p.Description, map => map.Length(1000));
            Property(p => p.Required);
            Property(p => p.ShowInDiscoveryDocument, map => map.NotNullable(true));
            Property(p => p.Emphasize);
            Property(p => p.Enabled);

            Set(p => p.UserClaims, map => 
            {
                map.Key(fk =>
                {
                    fk.Column("IdentityResourceId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_IdentityClaims_IdentityResource");
                });
                map.Fetch(CollectionFetchMode.Join);
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(IdentityResourceClaim)))
            );

            Set(p => p.Properties, map =>
            {
                map.Key(fk =>
                {
                    fk.Column("IdentityResourceId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_IdentityResourceProperties_IdentityResource");
                });
                map.Fetch(CollectionFetchMode.Join);
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(IdentityResourceProperty)))
            );
        }
    }
}
