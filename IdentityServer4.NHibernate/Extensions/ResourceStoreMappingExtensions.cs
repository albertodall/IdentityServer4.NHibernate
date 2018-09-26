namespace IdentityServer4.NHibernate.Extensions
{
    using Mappings.Entities;
    using AutoMapper;

    public static class ResourceStoreMappingExtensions
    {
        private static IMapper Mapper;

        static ResourceStoreMappingExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ResourceStoreMappingProfile>())
                .CreateMapper();
        }

        /// <summary>
        /// Maps an "ApiResource" entity to a "ApiResource" model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public static Models.ApiResource ToModel(this Entities.ApiResource entity)
        {
            return Mapper.Map<Models.ApiResource>(entity);
        }

        /// <summary>
        /// Maps a "ApiResource" model to an "ApiResource" entity.
        /// </summary>
        /// <param name="model">The model.</param>
        public static Entities.ApiResource ToEntity(this Models.ApiResource model)
        {
            return Mapper.Map<Entities.ApiResource>(model);
        }

        /// <summary>
        /// Maps an "IdentityResource" entity to a "IdentityResource" model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public static Models.IdentityResource ToModel(this Entities.IdentityResource entity)
        {
            return Mapper.Map<Models.IdentityResource>(entity);
        }

        /// <summary>
        /// Maps a "IdentityResource" model to an "IdentityResource" entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.IdentityResource ToEntity(this Models.IdentityResource model)
        {
            return Mapper.Map<Entities.IdentityResource>(model);
        }
    }
}
