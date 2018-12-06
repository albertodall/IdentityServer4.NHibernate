namespace IdentityServer4.NHibernate.Storage.Entities
{
    public class ClientCorsOrigin : EntityBase<int>
    {
        public virtual string Origin { get; set; }
    }
}