namespace IdentityServer4.NHibernate.Entities
{
    using System.Collections.Generic;

    public class ApiResource : EntityBase<int>
    {
        private readonly List<ApiSecret> _apiSecrets = new List<ApiSecret>();
        private readonly List<ApiScope> _apiScopes = new List<ApiScope>();
        private readonly List<ApiResourceClaim> _apiResourceClaims = new List<ApiResourceClaim>();

        public virtual bool Enabled { get; set; } = true;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual IEnumerable<ApiSecret> Secrets { get; }
        public virtual IEnumerable<ApiScope> Scopes { get; }
        public virtual IEnumerable<ApiResourceClaim> UserClaims { get; }
    }
}
