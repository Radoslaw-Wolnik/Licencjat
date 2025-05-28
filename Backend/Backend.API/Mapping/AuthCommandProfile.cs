using AutoMapper;
using Backend.API.DTOs.Auth;
using Backend.Application.Commands.Auth;

namespace Backend.API.Mapping;

public sealed class AuthCommandProfile : Profile
{
    public AuthCommandProfile()
    {
        CreateMap<RegisterRequest, RegisterCommand>();
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<ForgotPasswordRequest, ForgotCommand>();
    }
}