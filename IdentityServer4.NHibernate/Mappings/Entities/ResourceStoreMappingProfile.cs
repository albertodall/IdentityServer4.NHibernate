namespace IdentityServer4.NHibernate.Mappings.Entities
{
    using AutoMapper;

    /// <summary>
    /// Defines entity/model mapping for API resources and Identity resources.
    /// </summary>
    internal class ResourceStoreMappingProfile : Profile
    {
        public ResourceStoreMappingProfile()
        {
            CreateMap<NHibernate.Entities.ApiResource, Models.ApiResource>(MemberList.Destination)
                .ConstructUsing(src => new Models.ApiResource())
                .ForMember(x => x.ApiSecrets, opts => opts.MapFrom(x => x.Secrets))
                .ReverseMap();

            CreateMap<NHibernate.Entities.ApiResourceClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ApiSecret, Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();

            CreateMap<NHibernate.Entities.ApiScope, Models.Scope>(MemberList.Destination)
                .ConstructUsing(src => new Models.Scope())
                .ReverseMap();

            CreateMap<NHibernate.Entities.ApiScopeClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.IdentityResource, Models.IdentityResource>(MemberList.Destination)
                .ConstructUsing(src => new Models.IdentityResource())
                .ReverseMap();

            CreateMap<NHibernate.Entities.IdentityClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));
        }
    }
}
