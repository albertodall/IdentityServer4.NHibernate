using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using IdentityServer4.Models;

namespace IdentityServer4.NHibernate.Mappings.Entities
{
    /// <summary>
    /// Entity to model mapping (and vice-versa) for clients.
    /// </summary>
    internal class ClientStoreMappingProfile : Profile
    {
        public ClientStoreMappingProfile()
        {
            CreateMap<NHibernate.Entities.ClientProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<NHibernate.Entities.Client, Models.Client>()
                .ForMember(dest => dest.ProtocolType, opt => opt.Condition(srs => srs != null))
                .ForMember(
                    dest => dest.AllowedIdentityTokenSigningAlgorithms, 
                    opts => opts.ConvertUsing(new AllowedSigningAlgorithmsConverter(), src => src.AllowedIdentityTokenSigningAlgorithms))
                .ReverseMap()
                    .ForMember(
                        dest => dest.AllowedIdentityTokenSigningAlgorithms, 
                        opts => opts.ConvertUsing(new AllowedSigningAlgorithmsConverter(), src => src.AllowedIdentityTokenSigningAlgorithms));

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

            CreateMap<NHibernate.Entities.ClientSecret, Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();

            CreateMap<NHibernate.Entities.ClientIdPRestriction, string>()
                .ConstructUsing(src => src.Provider)
                .ReverseMap()
                    .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientClaim, ClientClaim>(MemberList.None)
                .ConstructUsing(src => new ClientClaim(src.Type, src.Value, ClaimValueTypes.String))
                .ReverseMap();

            CreateMap<NHibernate.Entities.ClientCorsOrigin, string>()
                .ConstructUsing(src => src.Origin)
                .ReverseMap()
                    .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src));
        }
    }
}
