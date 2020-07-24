using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings.Stores.Configuration
{
    internal class ClientMap : ClassMapping<Client>
    {
        public ClientMap()
        {
            Id(p => p.ID);

            Property(p => p.ClientId, map => 
            {
                map.Length(200);
                map.NotNullable(true);
                map.UniqueKey("UK_ClientId");
            });

            Property(p => p.ProtocolType, map => 
            {
                map.Length(200);
                map.NotNullable(true);
            });

            Property(p => p.Enabled, map => map.NotNullable(true));
            Property(p => p.ClientName, map => map.Length(200));
            Property(p => p.ClientUri, map => map.Length(2000));
            Property(p => p.LogoUri, map => map.Length(2000));
            Property(p => p.Description, map => map.Length(1000));
            Property(p => p.FrontChannelLogoutUri, map => map.Length(2000));
            Property(p => p.BackChannelLogoutUri, map => map.Length(2000));
            Property(p => p.ClientClaimsPrefix, map => map.Length(200));
            Property(p => p.UserCodeType, map => map.Length(100));
            Property(p => p.PairWiseSubjectSalt, map => map.Length(200));
            Property(p => p.AllowedIdentityTokenSigningAlgorithms, map => map.Length(100));
            Property(p => p.RequireClientSecret, map => map.NotNullable(true));
            Property(p => p.RequireConsent, map => map.NotNullable(true));
            Property(p => p.AllowRememberConsent, map => map.NotNullable(true));
            Property(p => p.AlwaysIncludeUserClaimsInIdToken, map => map.NotNullable(true));
            Property(p => p.RequirePkce, map => map.NotNullable(true));
            Property(p => p.AllowPlainTextPkce, map => map.NotNullable(true));
            Property(p => p.AllowAccessTokensViaBrowser, map => map.NotNullable(true));
            Property(p => p.FrontChannelLogoutUri, map => map.Length(2000));
            Property(p => p.FrontChannelLogoutSessionRequired, map => map.NotNullable(true));
            Property(p => p.BackChannelLogoutUri, map => map.Length(2000));
            Property(p => p.BackChannelLogoutSessionRequired, map => map.NotNullable(true));
            Property(p => p.AllowOfflineAccess, map => map.NotNullable(true));
            Property(p => p.IdentityTokenLifetime, map => map.NotNullable(true));
            Property(p => p.AccessTokenLifetime, map => map.NotNullable(true));
            Property(p => p.AuthorizationCodeLifetime, map => map.NotNullable(true));
            Property(p => p.ConsentLifetime);
            Property(p => p.AbsoluteRefreshTokenLifetime, map => map.NotNullable(true));
            Property(p => p.SlidingRefreshTokenLifetime, map => map.NotNullable(true));
            Property(p => p.RefreshTokenUsage, map => map.NotNullable(true));
            Property(p => p.EnableLocalLogin, map => map.NotNullable(true));
            Property(p => p.IncludeJwtId, map => map.NotNullable(true));
            Property(p => p.AlwaysSendClientClaims, map => map.NotNullable(true));
            Property(p => p.UpdateAccessTokenClaimsOnRefresh, map => map.NotNullable(true));
            Property(p => p.RefreshTokenExpiration);
            Property(p => p.AccessTokenType);

            Set(p => p.AllowedGrantTypes, map =>
            {
                map.Key(fk => 
                {
                    fk.Column("ClientId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ClientGrantTypes_Client");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            }, 
                r => r.OneToMany(m => m.Class(typeof(ClientGrantType)))
            );

            Set(p => p.ClientSecrets, map =>
            {
                map.Key(fk =>
                {
                    fk.Column("ClientId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ClientSecrets_Client");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ClientSecret)))
            );

            Set(p => p.RedirectUris, map =>
            {
                map.Key(fk => 
                {
                    fk.Column("ClientId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ClientRedirectUris_Client");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ClientRedirectUri)))
            );

            Set(p => p.PostLogoutRedirectUris, map =>
            {
                map.Key(fk =>
                {
                    fk.Column("ClientId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ClientPostLogoutRedirectUris_Client");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ClientPostLogoutRedirectUri)))
            );

            Set(p => p.AllowedScopes, map => 
            {
                map.Key(fk => 
                {
                    fk.Column("ClientId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ClientScope_Client");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ClientScope)))
            );

            Set(p => p.IdentityProviderRestrictions, map =>
            {
                map.Key(fk =>
                {
                    fk.Column("ClientId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ClientProviderRestrictions_Client");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ClientIdPRestriction)))
            );

            Set(p => p.Claims, map =>
            {
                map.Key(fk =>
                {
                    fk.Column("ClientId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ClientClaims_Client");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ClientClaim)))
            );

            Set(p => p.AllowedCorsOrigins, map =>
            {
                map.Key(fk =>
                {
                    fk.Column("ClientId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ClientCorsOrigin_Client");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ClientCorsOrigin)))
            );

            Set(p => p.Properties, map => 
            {
                map.Key(fk =>
                {
                    fk.Column("ClientId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ClientProperties_Client");
                });
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ClientProperty)))
            );
        }
    }
}
