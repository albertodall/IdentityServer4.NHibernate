using IdentityServer4.NHibernate.Mappings.Entities;
using AutoMapper;

namespace IdentityServer4.NHibernate.Extensions
{
    /// <summary>
    /// Add mapping profile for <see cref="Entities.PersistedGrant"/> to AutoMapper configuration.
    /// </summary>
    public static class PersistedGrantStoreMappingExtensions
    {
        private static IMapper Mapper;

        static PersistedGrantStoreMappingExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<PersistedGrantStoreMappingProfile>())
                .CreateMapper();
        }

        /// <summary>
        /// Maps an "PersistedGrant" entity to a "PersistedGrant" model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public static Models.PersistedGrant ToModel(this Entities.PersistedGrant entity)
        {
            return entity == null ? null : Mapper.Map<Models.PersistedGrant>(entity);
        }

        /// <summary>
        /// Maps a "PersistedGrant" model to a "PersistedGrant" entity.
        /// </summary>
        /// <param name="model">The model.</param>
        public static Entities.PersistedGrant ToEntity(this Models.PersistedGrant model)
        {
            return model == null ? null : Mapper.Map<Entities.PersistedGrant>(model);
        }

        /// <summary>
        /// Updates an entity from a model.
        /// </summary>
        /// <param name="model">The source model.</param>
        /// <param name="entity">The entity to update.</param>
        public static void UpdateEntity(this Models.PersistedGrant model, Entities.PersistedGrant entity)
        {
            Mapper.Map(model, entity);
        }
    }
}
