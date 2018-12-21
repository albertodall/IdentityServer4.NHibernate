namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public class ClientRedirectUri : EntityBase<int>
    {
        public virtual string RedirectUri { get; set; }
    }
}