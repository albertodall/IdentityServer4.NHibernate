namespace IdentityServer4.NHibernate.Entities
{
    using System;

    public class PersistedGrant : EntityBase<string>
    {
        public virtual string Type { get; set; }
        public virtual string SubjectId { get; set; }
        public virtual string ClientId { get; set; }
        public virtual DateTime CreationTime { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual string Data { get; set; }

    }
}
