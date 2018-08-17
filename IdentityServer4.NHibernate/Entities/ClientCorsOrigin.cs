namespace IdentityServer4.NHibernate.Entities
{
    public class ClientCorsOrigin : EntityBase<int>
    {
        public virtual string Origin { get; set; }
        public virtual Client Client { get; set; }
    }
}