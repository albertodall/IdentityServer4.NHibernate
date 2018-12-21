namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public class ClientPostLogoutRedirectUri : EntityBase<int>
    {
        public virtual string PostLogoutRedirectUri { get; set; }
    }
}