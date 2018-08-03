namespace IdentityServer4.NHibernate.Entities
{
    public class IdentityClaim : UserClaim
    {
        public virtual IdentityResource IdentityResource { get; set; }
    }
}
