using AutoMapper;
using IdentityServer4.Models;
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

            CreateMap<NHibernate.Entities.ApiResource, Models.ApiResource>(MemberList.Destination)
                .ConstructUsing(src => new Models.ApiResource())
                .ForMember(dest => dest.ApiSecrets, opt => opt.MapFrom(src => src.Secrets))
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
                    })
                    .ForMember(dest => dest.Properties, opt =>
                    {
                        opt.MapFrom(src => src.Properties);
                        opt.UseDestinationValue();
                    });

            CreateMap<ApiResourceClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<ApiSecret, Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();

            CreateMap<ApiScope, Scope>(MemberList.Destination)
                .ConstructUsing(src => new Scope())
                .ReverseMap()
                    .ForMember(dest => dest.UserClaims, opt =>
                    {
                        opt.MapFrom(src => src.UserClaims);
                        opt.UseDestinationValue();
                    });

            CreateMap<ApiScopeClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<IdentityResourceProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<NHibernate.Entities.IdentityResource, Models.IdentityResource>(MemberList.Destination)
                .ConstructUsing(src => new Models.IdentityResource())
                .ReverseMap()
                    .ForMember(dest => dest.UserClaims, opt => 
                    {
                        opt.MapFrom(src => src.UserClaims);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.Properties, opt =>
                    {
                        opt.MapFrom(src => src.Properties);
                        opt.UseDestinationValue();
                    });

            CreateMap<IdentityClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));
        }
    }
}
