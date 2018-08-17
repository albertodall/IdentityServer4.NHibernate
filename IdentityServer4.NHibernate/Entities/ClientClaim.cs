namespace IdentityServer4.NHibernate.Entities
{
    public class ClientClaim : EntityBase<int>
    {
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }
        public virtual Client Client { get; set; }
    }
}