namespace IdentityServer4.NHibernate.Mappings.Entities
{
    using AutoMapper;

    /// <summary>
    /// Entity/model mapping for clients.
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
                    });

            CreateMap<NHibernate.Entities.ClientGrantType, string>()
                .ConstructUsing(src => src.GrantType)
                .ReverseMap()
                    .ForMember(dest => dest.GrantType, opt => opt.MapFrom(src => src));

            CreateMap<NHibernate.Entities.ClientSecret, Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();
        }
    }
}
