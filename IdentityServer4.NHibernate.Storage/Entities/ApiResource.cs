using System;
using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public class ApiResource : EntityBase<int>
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual string AllowedAccessTokenSigningAlgorithms { get; set; }
        public virtual bool ShowInDiscoveryDocument { get; set; } = true;
        public virtual ISet<ApiResourceSecret> Secrets { get; set; } = new HashSet<ApiResourceSecret>();
        public virtual ISet<ApiResourceScope> Scopes { get; set; } = new HashSet<ApiResourceScope>();
        public virtual ISet<ApiResourceClaim> UserClaims { get; set; } = new HashSet<ApiResourceClaim>();
        public virtual ISet<ApiResourceProperty> Properties { get; set; } = new HashSet<ApiResourceProperty>();
        public virtual DateTime Created { get; set; } = DateTime.UtcNow;
        public virtual DateTime? Updated { get; set; }
        public virtual DateTime? LastAccessed { get; set; }
        public virtual bool NonEditable { get; set; }
    }
}
