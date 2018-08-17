namespace IdentityServer4.NHibernate.Entities
{
    public class ClientProperty : EntityBase<int>
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual Client Client { get; set; }
    }
}