namespace IdentityServer4.NHibernate.Entities
{
    public class ClientPostLogoutRedirectUri : EntityBase<int>
    {
        public virtual string PostLogoutRedirectUri { get; set; }
        public virtual Client Client { get; set; }
    }
}