namespace IdentityServer4.NHibernate.Entities
{
    using System.Collections.Generic;

    public class IdentityResource : EntityBase<int>
    {
        private readonly ICollection<IdentityClaim> _userClaims = new List<IdentityClaim>();

        public virtual bool Enabled { get; set; } = true;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Required { get; set; }
        public virtual bool Emphasize { get; set; }
        public virtual bool ShowInDiscoveryDocument { get; set; } = true;
        public virtual IEnumerable<IdentityClaim> UserClaims { get { return _userClaims; } }
    }
}