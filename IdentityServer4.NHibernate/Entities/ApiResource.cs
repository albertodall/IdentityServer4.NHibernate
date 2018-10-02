using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Entities
{
    public class ApiResource : EntityBase<int>
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual ISet<ApiSecret> Secrets { get; set; } = new HashSet<ApiSecret>();
        public virtual ISet<ApiScope> Scopes { get; set; } = new HashSet<ApiScope>();
        public virtual ISet<ApiResourceClaim> UserClaims { get; set; } = new HashSet<ApiResourceClaim>();
    }
}
