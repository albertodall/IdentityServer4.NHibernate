namespace IdentityServer4.NHibernate.Storage.Entities
{
    public class ClientClaim : EntityBase<int>
    {
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }
    }
}