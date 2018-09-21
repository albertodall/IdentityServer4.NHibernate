namespace IdentityServer4.NHibernate.Entities
{
    using System.Collections.Generic;

    public class ApiResource : EntityBase<int>
    {
        private readonly ICollection<ApiSecret> _secrets = new List<ApiSecret>();
        private readonly ICollection<ApiScope> _scopes = new List<ApiScope>();
        private readonly ICollection<ApiResourceClaim> _userClaims = new List<ApiResourceClaim>();

        public virtual bool Enabled { get; set; } = true;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual IEnumerable<ApiSecret> Secrets { get { return _secrets; } }
        public virtual IEnumerable<ApiScope> Scopes { get { return _scopes; } }
        public virtual IEnumerable<ApiResourceClaim> UserClaims { get { return _userClaims; } }
    }
}
