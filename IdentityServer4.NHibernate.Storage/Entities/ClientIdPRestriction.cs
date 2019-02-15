namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public class ClientIdPRestriction : EntityBase<int>
    {
        public virtual string Provider { get; set; }
    }
}