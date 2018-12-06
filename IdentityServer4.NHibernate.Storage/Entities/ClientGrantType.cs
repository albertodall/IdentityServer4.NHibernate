namespace IdentityServer4.NHibernate.Storage.Entities
{
    public class ClientGrantType : EntityBase<int>
    {
        public virtual string GrantType { get; set; }
    }
}