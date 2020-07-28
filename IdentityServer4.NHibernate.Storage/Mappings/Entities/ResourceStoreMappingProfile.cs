using AutoMapper;
using IdentityServer4.NHibernate.Entities;
using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Mappings.Entities
{
    /// <summary>
    /// Defines entity/model mapping for API resources and Identity resources.
    /// </summary>
    internal class ResourceStoreMappingProfile : Profile
    {
        public ResourceStoreMappingProfile()
        {
            CreateMap<ApiResourceProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<ApiResource, Models.ApiResource>(MemberList.Destination)
                .ConstructUsing(src => new Models.ApiResource())
                .ForMember(dst => dst.ApiSecrets, opt => opt.MapFrom(src => src.Secrets))
                .ForMember(
                    dst => dst.AllowedAccessTokenSigningAlgorithms, 
                    opt => opt.ConvertUsing(new AllowedSigningAlgorithmsConverter(), src => src.AllowedAccessTokenSigningAlgorithms))
                .ReverseMap()
                    .ForMember(
                        dst => dst.AllowedAccessTokenSigningAlgorithms, 
                        opt => opt.ConvertUsing(new AllowedSigningAlgorithmsConverter(), src => src.AllowedAccessTokenSigningAlgorithms));

            CreateMap<ApiResourceClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dst => dst.Type, opt => opt.MapFrom(src => src));

            CreateMap<ApiResourceSecret, Models.Secret>(MemberList.Destination)
                .ForMember(dst => dst.Type, opt => opt.Condition(src => src != null))
                .ReverseMap();

            CreateMap<ApiResourceScope, string>()
                .ConstructUsing(src => src.Scope)
                .ReverseMap()
                    .ForMember(dst => dst.Scope, opt => opt.MapFrom(src => src));

            CreateMap<ApiScopeClaim, string>()
                .ConstructUsing(src => src.Type)
                .ReverseMap()
                    .ForMember(dst => dst.Type, opt => opt.MapFrom(src => src));

            CreateMap<ApiScope, Models.ApiScope>(MemberList.Destination)
                .ConstructUsing(src => new Models.ApiScope())
                .ForMember(dst => dst.Properties, opt => opt.MapFrom(src => src.Properties))
                .ForMember(dst => dst.UserClaims, opt => opt.MapFrom(src => src.UserClaims))
                .ReverseMap();

            CreateMap<IdentityResourceProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<IdentityResource, Models.IdentityResource>(MemberList.Destination)
                .ConstructUsing(src => new Models.IdentityResource())
                .ReverseMap();

            CreateMap<IdentityResourceClaim, string>()
                .ConstructUsing(src => src.Type)
                .ReverseMap()
                    .ForMember(dst => dst.Type, opt => opt.MapFrom(src => src));
        }
    }
}
