namespace IdentityServer4.NHibernate.Entities
{
    public class ClientGrantType : EntityBase<int>
    {
        public string GrantType { get; set; }
        public virtual Client Client { get; set; }
    }
}