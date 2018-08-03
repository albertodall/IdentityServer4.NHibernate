namespace IdentityServer4.NHibernate.Entities
{
    public class ClientClaim : EntityBase<int>
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public virtual Client Client { get; set; }
    }
}