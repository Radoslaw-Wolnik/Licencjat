using Backend.Application.DTOs.Auth;
using Backend.Application.Features.Auth;
using AutoMapper;
using Backend.Domain.Entities;
using Backend.Domain.Errors;

namespace Backend.Infrastructure.Mapping;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<RegisterRequest, RegisterCommand>();
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<User, LoginResponse>()
            .ConstructUsing(src => new LoginResponse(
                src.Id, 
                src.Username
            ));
    }
}