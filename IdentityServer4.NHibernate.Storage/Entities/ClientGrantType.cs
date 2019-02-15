namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public class ClientGrantType : EntityBase<int>
    {
        public virtual string GrantType { get; set; }
    }
}