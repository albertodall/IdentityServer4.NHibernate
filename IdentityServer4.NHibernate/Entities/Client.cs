namespace IdentityServer4.NHibernate.Entities
{
    using IdentityServer4.Models;
    using System.Collections.Generic;

    public class Client : EntityBase<int>
    {
        private readonly List<ClientSecret> _clientSecrets = new List<ClientSecret>();
        private readonly List<ClientGrantType> _clientGrantTypes = new List<ClientGrantType>();
        private readonly List<ClientRedirectUri> _clientRedirectUris = new List<ClientRedirectUri>();
        private readonly List<ClientPostLogoutRedirectUri> _clientPostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>();
        private readonly List<ClientScope> _clientScopes = new List<ClientScope>();
        private readonly List<ClientIdPRestriction> _clientIdPRestrictions = new List<ClientIdPRestriction>();
        private readonly List<ClientClaim> _clientClaims = new List<ClientClaim>();
        private readonly List<ClientCorsOrigin> _clientCorsOrigins = new List<ClientCorsOrigin>();
        private readonly List<ClientProperty> _clientProperties = new List<ClientProperty>();

        public virtual bool Enabled { get; set; } = true;
        public virtual string ClientId { get; set; }
        public virtual string ProtocolType { get; set; } = IdentityServerConstants.ProtocolTypes.OpenIdConnect;
        public virtual IEnumerable<ClientSecret> ClientSecrets { get; }
        public virtual bool RequireClientSecret { get; set; } = true;
        public virtual string ClientName { get; set; }
        public virtual string Description { get; set; }
        public virtual string ClientUri { get; set; }
        public virtual string LogoUri { get; set; }
        public virtual bool RequireConsent { get; set; } = true;
        public virtual bool AllowRememberConsent { get; set; } = true;
        public virtual bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public virtual IEnumerable<ClientGrantType> AllowedGrantTypes { get; }
        public virtual bool RequirePkce { get; set; }
        public virtual bool AllowPlainTextPkce { get; set; }
        public virtual bool AllowAccessTokensViaBrowser { get; set; }
        public virtual IEnumerable<ClientRedirectUri> RedirectUris { get; }
        public virtual IEnumerable<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; }
        public virtual string FrontChannelLogoutUri { get; set; }
        public virtual bool FrontChannelLogoutSessionRequired { get; set; } = true;
        public virtual string BackChannelLogoutUri { get; set; }
        public virtual bool BackChannelLogoutSessionRequired { get; set; } = true;
        public virtual bool AllowOfflineAccess { get; set; }
        public virtual IEnumerable<ClientScope> AllowedScopes { get; set; }
        public virtual int IdentityTokenLifetime { get; set; } = 300;
        public virtual int AccessTokenLifetime { get; set; } = 3600;
        public virtual int AuthorizationCodeLifetime { get; set; } = 300;
        public virtual int? ConsentLifetime { get; set; } = null;
        public virtual int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public virtual int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public virtual int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;
        public virtual bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public virtual int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;
        public virtual int AccessTokenType { get; set; } = (int)Models.AccessTokenType.Jwt;
        public virtual bool EnableLocalLogin { get; set; } = true;
        public virtual IEnumerable<ClientIdPRestriction> IdentityProviderRestrictions { get; }
        public virtual bool IncludeJwtId { get; set; }
        public virtual IEnumerable<ClientClaim> Claims { get; }
        public virtual bool AlwaysSendClientClaims { get; set; }
        public virtual string ClientClaimsPrefix { get; set; } = "client_";
        public virtual string PairWiseSubjectSalt { get; set; }
        public virtual IEnumerable<ClientCorsOrigin> AllowedCorsOrigins { get; }
        public virtual IEnumerable<ClientProperty> Properties { get; }
    }
}
