namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public class ClientScope : EntityBase<int>
    {
        public virtual string Scope { get; set; }
    }
}