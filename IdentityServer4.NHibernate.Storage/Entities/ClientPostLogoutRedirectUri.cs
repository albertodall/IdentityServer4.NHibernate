namespace IdentityServer4.NHibernate.Storage.Entities
{
    public class ClientPostLogoutRedirectUri : EntityBase<int>
    {
        public virtual string PostLogoutRedirectUri { get; set; }
    }
}