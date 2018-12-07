namespace IdentityServer4.NHibernate.Entities
{
    public interface IEntity<out TId>
    {
        TId ID { get; }
        bool IsTransient();
    }
}
