namespace IdentityServer4.NHibernate.Storage.Entities
{
    public class ClientIdPRestriction : EntityBase<int>
    {
        public virtual string Provider { get; set; }
    }
}