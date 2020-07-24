﻿using System;
using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public class IdentityResource : EntityBase<int>
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Required { get; set; }
        public virtual bool Emphasize { get; set; }
        public virtual bool ShowInDiscoveryDocument { get; set; } = true;
        public virtual ISet<IdentityResourceClaim> UserClaims { get; set; } = new HashSet<IdentityResourceClaim>();
        public virtual ISet<IdentityResourceProperty> Properties { get; set; } = new HashSet<IdentityResourceProperty>();
        public virtual DateTime Created { get; set; } = DateTime.UtcNow;
        public virtual DateTime? Updated { get; set; }
        public virtual bool NonEditable { get; set; }
    }
}