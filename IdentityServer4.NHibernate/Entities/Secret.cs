namespace IdentityServer4.NHibernate.Entities
{
    using System;

    public abstract class Secret : EntityBase<int>
    {
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; } = IdentityServerConstants.SecretTypes.SharedSecret;
    }
}
