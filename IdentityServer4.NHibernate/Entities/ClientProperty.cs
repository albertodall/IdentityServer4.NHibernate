namespace IdentityServer4.NHibernate.Entities
{
    public class ClientProperty : EntityBase<int>
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public virtual Client Client { get; set; }
    }
}