using System;

namespace IdentityServer4.NHibernate.Entities
{
    public abstract class Secret : EntityBase<int>
    {
        public virtual string Description { get; set; }
        public virtual string Value { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual string Type { get; set; } = IdentityServerConstants.SecretTypes.SharedSecret;
    }
}
