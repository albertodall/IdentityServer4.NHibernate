using AutoMapper;

namespace IdentityServer4.NHibernate.Storage.Mappings.Entities
{
    /// <summary>
    /// Entity to model mapping (and vice-versa) for persisted grants.
    /// </summary>
    internal class PersistedGrantStoreMappingProfile : Profile
    {
        public PersistedGrantStoreMappingProfile()
        {
            CreateMap<NHibernate.Storage.Entities.PersistedGrant, Models.PersistedGrant>(MemberList.Destination)
                .ReverseMap()
                    .ForMember(dest => dest.ID, map => map.MapFrom(src => src.Key));
        }
    }
}