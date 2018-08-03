namespace IdentityServer4.NHibernate.Entities
{
    public class ClientRedirectUri : EntityBase<int>
    {
        public string RedirectUri { get; set; }
        public virtual Client Client { get; set; }
    }
}