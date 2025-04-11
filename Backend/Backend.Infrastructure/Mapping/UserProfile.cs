// Backend.Infrastructure/Mapping/UserProfile.cs
using AutoMapper;
using Backend.Infrastructure.Entities;
using Backend.Domain.Entities;

namespace Backend.Infrastructure.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<ApplicationUser, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
            // .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ReverseMap(); // For bidirectional mapping
        
    }
}