using AutoMapper;
using Backend.API.DTOs.Auth;
using Backend.Application.Commands.Auth;

namespace Backend.API.Mapping;

public sealed class AuthCommandProfile : Profile
{
    public AuthCommandProfile()
    {
        CreateMap<RegisterRequest, RegisterCommand>()
            .ForCtorParam("Email", opt => opt.MapFrom(src => src.Email))
            .ForCtorParam("Username", opt => opt.MapFrom(src => src.Username))
            .ForCtorParam("Password", opt => opt.MapFrom(src => src.Password))
            .ForCtorParam("FirstName", opt => opt.MapFrom(src => src.FirstName))
            .ForCtorParam("LastName", opt => opt.MapFrom(src => src.LastName))
            .ForCtorParam("BirthDate", opt => opt.MapFrom(src => src.BirthDate))
            .ForCtorParam("City", opt => opt.MapFrom(src => src.City))
            .ForCtorParam("Country", opt => opt.MapFrom(src => src.Country));
        // .ForAllMembers(opt => opt.Ignore());

        CreateMap<LoginRequest, LoginCommand>()
            .ForCtorParam("UsernameOrEmail", opt => opt.MapFrom(src => src.UsernameOrEmail))
            .ForCtorParam("Password", opt => opt.MapFrom(src => src.Password))
            .ForCtorParam("RememberMe", opt => opt.MapFrom(src => src.RememberMe));


        CreateMap<ForgotPasswordRequest, ForgotCommand>()
            .ForCtorParam("Email", opt => opt.MapFrom(src => src.Email));
    }
}