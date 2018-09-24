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
                .ReverseMap()
                    .ForMember(dest => dest.Scopes, opt => 
                    {
                        opt.MapFrom(src => src.Scopes);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.Secrets, opt => 
                    {
                        opt.MapFrom(src => src.ApiSecrets);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.UserClaims, opt => 
                    {
                        opt.MapFrom(src => src.UserClaims);
                        opt.UseDestinationValue();
                    });

            CreateMap<NHibernate.Entities.ApiResourceClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ApiSecret, Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();

            CreateMap<NHibernate.Entities.ApiScope, Models.Scope>(MemberList.Destination)
                .ConstructUsing(src => new Models.Scope())
                .ReverseMap()
                    .ForMember(dest => dest.UserClaims, opt => 
                    {
                        opt.MapFrom(src => src.UserClaims);
                        opt.UseDestinationValue();
                    });

            CreateMap<NHibernate.Entities.ApiScopeClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.IdentityResource, Models.IdentityResource>(MemberList.Destination)
                .ConstructUsing(src => new Models.IdentityResource())
                .ReverseMap()
                    .ForMember(dest => dest.UserClaims, opt => 
                    {
                        opt.MapFrom(src => src.UserClaims);
                        opt.UseDestinationValue();
                    });

            CreateMap<NHibernate.Entities.IdentityClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));
        }
    }
}
