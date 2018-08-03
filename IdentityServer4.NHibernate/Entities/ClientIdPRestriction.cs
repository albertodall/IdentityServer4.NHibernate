namespace IdentityServer4.NHibernate.Entities
{
    public class ClientIdPRestriction : EntityBase<int>
    {
        public string Provider { get; set; }
        public virtual Client Client { get; set; }
    }
}