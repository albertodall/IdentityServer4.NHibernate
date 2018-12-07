namespace IdentityServer4.NHibernate.Entities
{
    public abstract class UserClaim : EntityBase<int>
    {
        public virtual string Type { get; set; }
    }
}
