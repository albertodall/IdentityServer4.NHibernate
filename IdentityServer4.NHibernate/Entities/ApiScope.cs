namespace IdentityServer4.NHibernate.Entities
{
    using System.Collections.Generic;

    public class ApiScope : EntityBase<int>
    {
        private readonly List<ApiScopeClaim> _userClaims = new List<ApiScopeClaim>();

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public virtual IEnumerable<ApiScopeClaim> UserClaims { get; }

        public virtual ApiResource ApiResource { get; set; }
    }
}