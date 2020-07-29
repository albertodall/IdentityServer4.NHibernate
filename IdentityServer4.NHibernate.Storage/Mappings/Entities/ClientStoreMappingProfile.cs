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
                .ForMember(dst => dst.ProtocolType, opt => opt.Condition(src => src != null))
                .ForMember(
                    dst => dst.AllowedIdentityTokenSigningAlgorithms, 
                    opt => opt.ConvertUsing(AllowedSigningAlgorithmsConverter.Instance, src => src.AllowedIdentityTokenSigningAlgorithms))
                .ReverseMap()
                    .ForMember(
                        dst => dst.AllowedIdentityTokenSigningAlgorithms, 
                        opt => opt.ConvertUsing(AllowedSigningAlgorithmsConverter.Instance, src => src.AllowedIdentityTokenSigningAlgorithms));

            CreateMap<NHibernate.Entities.ClientGrantType, string>()
                .ConstructUsing(src => src.GrantType)
                .ReverseMap()
                    .ForMember(dst => dst.GrantType, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientRedirectUri, string>()
                .ConstructUsing(src => src.RedirectUri)
                .ReverseMap()
                    .ForMember(dst => dst.RedirectUri, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientPostLogoutRedirectUri, string>()
                .ConstructUsing(src => src.PostLogoutRedirectUri)
                .ReverseMap()
                    .ForMember(dst => dst.PostLogoutRedirectUri, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientScope, string>()
                .ConstructUsing(src => src.Scope)
                .ReverseMap()
                    .ForMember(dst => dst.Scope, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientSecret, Secret>(MemberList.Destination)
                .ForMember(dst => dst.Type, opt => opt.Condition(src => src != null))
                .ReverseMap();

            CreateMap<NHibernate.Entities.ClientIdPRestriction, string>()
                .ConstructUsing(src => src.Provider)
                .ReverseMap()
                    .ForMember(dst => dst.Provider, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientClaim, ClientClaim>(MemberList.None)
                .ConstructUsing(src => new ClientClaim(src.Type, src.Value, ClaimValueTypes.String))
                .ReverseMap();

            CreateMap<NHibernate.Entities.ClientCorsOrigin, string>()
                .ConstructUsing(src => src.Origin)
                .ReverseMap()
                    .ForMember(dst => dst.Origin, opt => opt.MapFrom(src => src));
        }
    }
}
