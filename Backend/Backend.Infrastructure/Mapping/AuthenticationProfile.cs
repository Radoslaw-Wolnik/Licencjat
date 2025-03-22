// Backend.Infrastructure/Mapping/AuthenticationProfile.cs
using AutoMapper;
using Backend.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Mapping;

public class AuthenticationProfile : Profile
{
    public AuthenticationProfile()
    {
        CreateMap<SignInResult, AuthenticationResult>()
            .ForMember(dest => dest.Succeeded, opt => opt.MapFrom(src => src.Succeeded))
            .ForMember(dest => dest.RequiresTwoFactor, opt => opt.MapFrom(src => src.RequiresTwoFactor))
            .ForMember(dest => dest.IsLockedOut, opt => opt.MapFrom(src => src.IsLockedOut))
            .ForMember(dest => dest.ErrorMessage, opt => opt.Ignore()); // Handle errors separately
    }
}