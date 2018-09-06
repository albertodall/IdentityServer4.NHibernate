namespace IdentityServer4.NHibernate.Extensions
{
    using Mappings.Entities;
    using AutoMapper;

    internal static class ClientStoreMappingExtensions
    {
        private static IMapper Mapper;

        static ClientStoreMappingExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientStoreMappingProfile>()).CreateMapper();
        }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public static Models.Client ToModel(this Entities.Client entity)
        {
            return Mapper.Map<Models.Client>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        public static Entities.Client ToEntity(this Models.Client model)
        {
            return Mapper.Map<Entities.Client>(model);
        }
    }
}
