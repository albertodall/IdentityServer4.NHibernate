namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public class ClientClaim : EntityBase<int>
    {
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }
    }
}