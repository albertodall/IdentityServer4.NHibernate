namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public class ClientCorsOrigin : EntityBase<int>
    {
        public virtual string Origin { get; set; }
    }
}