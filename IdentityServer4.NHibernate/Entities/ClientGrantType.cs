namespace IdentityServer4.NHibernate.Entities
{
    public class ClientGrantType : EntityBase<int>
    {
        public virtual string GrantType { get; set; }
    }
}