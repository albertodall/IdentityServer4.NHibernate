namespace IdentityServer4.NHibernate.Entities
{
    public class ApiSecret : Secret
    {
        public virtual ApiResource ApiResource { get; set; }
    }
}