using System;

namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public abstract class Secret : EntityBase<int>
    {
        public virtual string Description { get; set; }
        public virtual string Value { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual string Type { get; set; } = "SharedSecret";
        public virtual DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
