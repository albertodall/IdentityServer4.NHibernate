using IdentityServer4.NHibernate.Mappings.Entities;
using AutoMapper;

namespace IdentityServer4.NHibernate.Extensions
{
    /// <summary>
    /// Add mapping profile for <see cref="Entities.ApiResource"/> and <see cref="Entities.IdentityResource"/> 
    /// to AutoMapper configuration.
    /// </summary>
    public static class ResourceStoreMappingExtensions
    {
        private static readonly IMapper Mapper;

        static ResourceStoreMappingExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ResourceStoreMappingProfile>())
                .CreateMapper();
        }

        /// <summary>
        /// Maps a "ApiResource" entity to a "ApiResource" model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public static Models.ApiResource ToModel(this Entities.ApiResource entity)
        {
            return entity == null ? null : Mapper.Map<Models.ApiResource>(entity);
        }

        /// <summary>
        /// Maps a "ApiResource" model to an "ApiResource" entity.
        /// </summary>
        /// <param name="model">The model.</param>
        public static Entities.ApiResource ToEntity(this Models.ApiResource model)
        {
            return model == null ? null : Mapper.Map<Entities.ApiResource>(model);
        }

        /// <summary>
        /// Maps a "IdentityResource" entity to a "IdentityResource" model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public static Models.IdentityResource ToModel(this Entities.IdentityResource entity)
        {
            return entity == null ? null : Mapper.Map<Models.IdentityResource>(entity);
        }

        /// <summary>
        /// Maps a "IdentityResource" model to an "IdentityResource" entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.IdentityResource ToEntity(this Models.IdentityResource model)
        {
            return model == null ? null : Mapper.Map<Entities.IdentityResource>(model);
        }

        /// <summary>
        /// Maps a "ApiScope" entity to a "ApiScope" model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public static Models.ApiScope ToModel(this Entities.ApiScope entity)
        {
            return entity == null ? null : Mapper.Map<Models.ApiScope>(entity);
        }

         /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.ApiScope ToEntity(this Models.ApiScope model)
        {
            return model == null ? null : Mapper.Map<Entities.ApiScope>(model);
        }
    }
}
