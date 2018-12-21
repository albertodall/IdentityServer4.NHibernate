using IdentityServer4.NHibernate.Mappings.Entities;
using AutoMapper;

namespace IdentityServer4.NHibernate.Extensions
{
    /// <summary>
    /// Add mapping profile for <see cref="Entities.Client"/> to AutoMapper configuration.
    /// </summary>
    public static class ClientStoreMappingExtensions
    {
        private static IMapper Mapper;

        static ClientStoreMappingExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientStoreMappingProfile>())
                .CreateMapper();
        }

        /// <summary>
        /// Maps an "Client" entity to a "Client" model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public static Models.Client ToModel(this Entities.Client entity)
        {
            return Mapper.Map<Models.Client>(entity);
        }

        /// <summary>
        /// Maps a "Client" model to a "Client" entity.
        /// </summary>
        /// <param name="model">The model.</param>
        public static Entities.Client ToEntity(this Models.Client model)
        {
            return Mapper.Map<Entities.Client>(model);
        }
    }
}
