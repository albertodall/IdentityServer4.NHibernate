namespace IdentityServer4.NHibernate.Entities
{
    using System.Collections.Generic;

    public class IdentityResource : EntityBase<int>
    {
        private readonly List<IdentityClaim> _userClaims = new List<IdentityClaim>();

        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public virtual IEnumerable<IdentityClaim> UserClaims { get; }
    }
}