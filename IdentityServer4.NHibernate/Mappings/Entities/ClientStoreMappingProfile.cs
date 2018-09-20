namespace IdentityServer4.NHibernate.Mappings.Entities
{
    using System.Security.Claims;
    using AutoMapper;

    /// <summary>
    /// Entity to model mapping (and vice-versa) for clients.
    /// </summary>
    internal class ClientStoreMappingProfile : Profile
    {
        public ClientStoreMappingProfile()
        {
            CreateMap<NHibernate.Entities.Client, Models.Client>()
                .ForMember(dest => dest.ProtocolType, opt => opt.Condition(srs => srs != null))
                .ReverseMap()
                    .ForMember(dest => dest.AllowedGrantTypes, opt => 
                    {
                        opt.MapFrom(src => src.AllowedGrantTypes);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.ClientSecrets, opt =>
                    {
                        opt.MapFrom(src => src.ClientSecrets);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.RedirectUris, opt =>
                    {
                        opt.MapFrom(src => src.RedirectUris);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.PostLogoutRedirectUris, opt =>
                    {
                        opt.MapFrom(src => src.PostLogoutRedirectUris);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.AllowedScopes, opt =>
                    {
                        opt.MapFrom(src => src.AllowedScopes);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.IdentityProviderRestrictions, opt =>
                    {
                        opt.MapFrom(src => src.IdentityProviderRestrictions);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.Claims, opt =>
                    {
                        opt.MapFrom(src => src.Claims);
                        opt.UseDestinationValue();
                    });

            CreateMap<NHibernate.Entities.ClientGrantType, string>()
                .ConstructUsing(src => src.GrantType)
                .ReverseMap()
                    .ForMember(dest => dest.GrantType, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientRedirectUri, string>()
                .ConstructUsing(src => src.RedirectUri)
                .ReverseMap()
                    .ForMember(dest => dest.RedirectUri, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientPostLogoutRedirectUri, string>()
                .ConstructUsing(src => src.PostLogoutRedirectUri)
                .ReverseMap()
                    .ForMember(dest => dest.PostLogoutRedirectUri, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientScope, string>()
                .ConstructUsing(src => src.Scope)
                .ReverseMap()
                    .ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientSecret, Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();

            CreateMap<NHibernate.Entities.ClientIdPRestriction, string>()
                .ConstructUsing(src => src.Provider)
                .ReverseMap()
                    .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientClaim, Claim>(MemberList.None)
                .ConstructUsing(src => new Claim(src.Type, src.Value))
                .ReverseMap();
        }
    }
}
