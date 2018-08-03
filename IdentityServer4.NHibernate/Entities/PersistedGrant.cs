namespace IdentityServer4.NHibernate.Entities
{
    using System;

    public class PersistedGrant : EntityBase<string>
    {
        public string Type { get; set; }
        public string SubjectId { get; set; }
        public string ClientId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? Expiration { get; set; }
        public string Data { get; set; }
    }
}
