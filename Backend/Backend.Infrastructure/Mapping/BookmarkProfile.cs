using AutoMapper;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;

namespace Backend.Infrastructure.Mapping;

public class BookmarkProfile : Profile
{

    public BookmarkProfile()
    {
        CreateMap<BookmarkEntity, Bookmark>()
            .ConstructUsing((src, _) =>
            {
            var result = Bookmark.Create(src.Id, src.UserBookId, src.Colour, src.Page, src.Description);
                if (result.IsFailed)
                    throw new AutoMapperMappingException($"Invalid bookmark: {string.Join(", ", result.Errors)}");
                return result.Value;
            })
            .ForAllMembers(opt => opt.Ignore());

        
        CreateMap<Bookmark, BookmarkEntity>()
            .ConstructUsing(src => new BookmarkEntity
            {
                Id = src.Id,
                UserBookId = src.UserBookId,
                Colour = src.Colour,
                Page = src.Page,
                Description = src.Description
            });    
        
    }

}