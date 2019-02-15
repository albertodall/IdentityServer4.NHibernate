namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public abstract class UserClaim : EntityBase<int>
    {
        public virtual string Type { get; set; }
    }
}
