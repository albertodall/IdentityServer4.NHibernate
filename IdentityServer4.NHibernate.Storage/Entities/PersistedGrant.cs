using System;

namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public class PersistedGrant : EntityBase<string>
    {
        public virtual string Type { get; set; }
        public virtual string SubjectId { get; set; }
        public virtual string SessionId { get; set; }
        public virtual string ClientId { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreationTime { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual DateTime? ConsumedTime { get; set; }
        public virtual string Data { get; set; }

    }
}
