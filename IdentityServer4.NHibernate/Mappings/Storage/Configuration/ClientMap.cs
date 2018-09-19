namespace IdentityServer4.NHibernate.Mappings.Storage.Configuration
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ClientMap : ClassMapping<Client>
    {
        public ClientMap()
        {
            Id(p => p.ID, map => 
            {
                map.Generator(Generators.Native);
                map.Column("Id");
            });

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
            Property(p => p.PairWiseSubjectSalt, map => map.Length(200));
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

            Set<ClientGrantType>("_allowedGrantTypes", map =>
            {
                map.Key(fk => 
                {
                    fk.Column("ClientId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ClientGrantType_Client");
                });
                map.Access(Accessor.Field);
                map.Fetch(CollectionFetchMode.Join);
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            }, 
                r => r.OneToMany(m => m.Class(typeof(ClientGrantType)))
            );

            Set<ClientSecret>("_clientSecrets", map =>
            {
                map.Key(fk =>
                {
                    fk.Column("ClientId");
                    fk.NotNullable(true);
                    fk.ForeignKey("FK_ClientSecret_Client");
                });
                map.Lazy(CollectionLazy.Lazy);
                map.Access(Accessor.Field);
                map.Fetch(CollectionFetchMode.Join);
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
            },
                r => r.OneToMany(m => m.Class(typeof(ClientSecret)))
            );

            // RedirectUris
            // PostLogoutRedirectUris
            // AllowedScopes
            // IdentityProviderRestrictions
            // Claims
            // AllowedCorsOrigins
            // Properties
        }
    }
}
