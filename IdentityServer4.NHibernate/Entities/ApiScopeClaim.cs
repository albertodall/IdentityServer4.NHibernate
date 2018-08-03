namespace IdentityServer4.NHibernate.Entities
{
    public class ApiScopeClaim : UserClaim
    {
        public virtual ApiScope ApiScope { get; set; }
    }
}