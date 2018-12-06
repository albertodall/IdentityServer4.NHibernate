namespace IdentityServer4.NHibernate.Storage.Entities
{
    public interface IEntity<out TId>
    {
        TId ID { get; }
        bool IsTransient();
    }
}
