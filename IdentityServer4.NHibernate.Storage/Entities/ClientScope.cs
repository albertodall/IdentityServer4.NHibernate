namespace IdentityServer4.NHibernate.Storage.Entities
{
    public class ClientScope : EntityBase<int>
    {
        public virtual string Scope { get; set; }
    }
}