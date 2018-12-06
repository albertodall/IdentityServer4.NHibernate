namespace IdentityServer4.NHibernate.Storage.Entities
{
    public class Property : EntityBase<int>
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
    }
}