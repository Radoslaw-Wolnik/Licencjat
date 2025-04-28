using AutoMapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using Backend.Application.DTOs;
using FluentResults;
using Backend.Domain.Enums;

namespace Backend.Infrastructure.Mapping;

public class BookmarkProfile : Profile
{
    public BookmarkProfile()
    {
        CreateMap<BookmarkEntity, Bookmark>()
            .ConstructUsing(src => new Bookmark(src.Id, src.UserBookId, src.Colour, src.Page, src.Description))
            .ReverseMap();
    }

}