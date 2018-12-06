namespace IdentityServer4.NHibernate.Storage.Entities
{
    public class ClientRedirectUri : EntityBase<int>
    {
        public virtual string RedirectUri { get; set; }
    }
}