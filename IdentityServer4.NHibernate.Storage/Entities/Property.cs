namespace IdentityServer4.NHibernate.Entities
{
    #pragma warning disable 1591

    public class Property : EntityBase<int>
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
    }
}