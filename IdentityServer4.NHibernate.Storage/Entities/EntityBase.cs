namespace IdentityServer4.NHibernate.Entities
{
    /// <summary>
    /// Base class for entities.
    /// </summary>
    /// <typeparam name="TId">Identifier type.</typeparam>
    public abstract class EntityBase<TId> : IEntity<TId>
    {
        private const int HashMultiplier = 29;

        private int? cachedHashCode;

        /// <summary>
        /// Unique identifier.
        /// </summary>
        public virtual TId ID { get; protected internal set; }

        /// <summary>
        /// Returns true if the entity is transient (not yet persisted in the database).
        /// </summary>
        public virtual bool IsTransient()
        {
            return ID == null || ID.Equals(default(TId));
        }

        #pragma warning disable 1591

        public override bool Equals(object obj)
        {
            if (!(obj is IEntity<TId> other)) return false;

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (IsTransient() && other.IsTransient())
            {
                return ReferenceEquals(this, other);
            }

            return other.ID.Equals(ID);
        }

        public override int GetHashCode()
        {
            if (cachedHashCode.HasValue)
            {
                return cachedHashCode.Value;
            }

            if (IsTransient())
            {
                cachedHashCode = base.GetHashCode();
            }
            else
            {
                cachedHashCode = (HashMultiplier * GetType().GetHashCode()) ^ ID.GetHashCode();
            }

            return cachedHashCode.Value;
        }

        public static bool operator == (EntityBase<TId> left, EntityBase<TId> right)
        {
            return Equals(left, right);
        }

        public static bool operator != (EntityBase<TId> left, EntityBase<TId> right)
        {
            return !Equals(left, right);
        }
    }
}
