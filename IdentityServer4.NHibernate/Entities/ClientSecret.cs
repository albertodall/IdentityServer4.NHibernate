namespace IdentityServer4.NHibernate.Entities
{
    public class ClientSecret : Secret
    {
        public virtual Client Client { get; set; }
    }
}