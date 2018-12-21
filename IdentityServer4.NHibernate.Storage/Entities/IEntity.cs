namespace IdentityServer4.NHibernate.Entities
{
    /// <summary>
    /// Base interface for entities.
    /// </summary>
    /// <typeparam name="TId">Unique identifier type.</typeparam>
    public interface IEntity<out TId>
    {
        /// <summary>
        /// Returns the value entity's unique identifier.
        /// </summary>
        TId ID { get; }

        /// <summary>
        /// Returns true if the entity is transient (not yet persisted in the database).
        /// </summary>
        bool IsTransient();
    }
}
