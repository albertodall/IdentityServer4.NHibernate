namespace IdentityServer4.NHibernate.Storage.Entities
{
    public abstract class UserClaim : EntityBase<int>
    {
        public virtual string Type { get; set; }
    }
}
