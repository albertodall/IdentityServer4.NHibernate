using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Entities
{
    public class ApiScope : EntityBase<int>
    {
        private readonly ICollection<ApiScopeClaim> _userClaims = new List<ApiScopeClaim>();

        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Required { get; set; }
        public virtual bool Emphasize { get; set; }
        public virtual bool ShowInDiscoveryDocument { get; set; } = true;
        public virtual IEnumerable<ApiScopeClaim> UserClaims { get { return _userClaims; } }

        // This property is not strictly necessary from NHibernate's standpoint, 
        // but allows us to write better queries.
        public virtual ApiResource ApiResource { get; set; }
    }
}