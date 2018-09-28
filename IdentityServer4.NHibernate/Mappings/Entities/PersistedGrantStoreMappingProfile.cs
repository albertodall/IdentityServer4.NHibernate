namespace IdentityServer4.NHibernate.Mappings.Entities
{
    using AutoMapper;

    /// <summary>
    /// Entity to model mapping (and vice-versa) for persisted grants.
    /// </summary>
    internal class PersistedGrantStoreMappingProfile : Profile
    {
        public PersistedGrantStoreMappingProfile()
        {
            CreateMap<NHibernate.Entities.PersistedGrant, Models.PersistedGrant>(MemberList.Destination)
                .ReverseMap()
                    .ForMember(dest => dest.ID, map => map.MapFrom(src => src.Key));
        }
    }
}