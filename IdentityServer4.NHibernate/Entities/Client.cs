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

        public bool Enabled { get; set; } = true;
        public string ClientId { get; set; }
        public string ProtocolType { get; set; } = IdentityServerConstants.ProtocolTypes.OpenIdConnect;
        public IEnumerable<ClientSecret> ClientSecrets { get; }
        public bool RequireClientSecret { get; set; } = true;
        public string ClientName { get; set; }
        public string Description { get; set; }
        public string ClientUri { get; set; }
        public string LogoUri { get; set; }
        public bool RequireConsent { get; set; } = true;
        public bool AllowRememberConsent { get; set; } = true;
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public virtual IEnumerable<ClientGrantType> AllowedGrantTypes { get; }
        public bool RequirePkce { get; set; }
        public bool AllowPlainTextPkce { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public virtual IEnumerable<ClientRedirectUri> RedirectUris { get; }
        public virtual IEnumerable<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; }
        public string FrontChannelLogoutUri { get; set; }
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;
        public string BackChannelLogoutUri { get; set; }
        public bool BackChannelLogoutSessionRequired { get; set; } = true;
        public bool AllowOfflineAccess { get; set; }
        public IEnumerable<ClientScope> AllowedScopes { get; set; }
        public int IdentityTokenLifetime { get; set; } = 300;
        public int AccessTokenLifetime { get; set; } = 3600;
        public int AuthorizationCodeLifetime { get; set; } = 300;
        public int? ConsentLifetime { get; set; } = null;
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;
        public int AccessTokenType { get; set; } = (int)Models.AccessTokenType.Jwt;
        public bool EnableLocalLogin { get; set; } = true;
        public virtual IEnumerable<ClientIdPRestriction> IdentityProviderRestrictions { get; }
        public bool IncludeJwtId { get; set; }
        public virtual IEnumerable<ClientClaim> Claims { get; }
        public bool AlwaysSendClientClaims { get; set; }
        public string ClientClaimsPrefix { get; set; } = "client_";
        public string PairWiseSubjectSalt { get; set; }
        public virtual IEnumerable<ClientCorsOrigin> AllowedCorsOrigins { get; }
        public virtual IEnumerable<ClientProperty> Properties { get; }
    }
}
