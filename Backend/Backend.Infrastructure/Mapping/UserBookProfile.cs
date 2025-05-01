using AutoMapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;

namespace Backend.Infrastructure.Mapping;

public class UserBookProfile : Profile
{
    public UserBookProfile()
    {
        
        // Entity -> Domain Mapping
        CreateMap<UserBookEntity, UserBook>(MemberList.None)
            .ConstructUsing((src, ctx) =>
            {
                // map your VOs and scalars:
                var languageCodeResult = LanguageCode.Create(src.Language);
                if (languageCodeResult.IsFailed)
                    throw new AutoMapperMappingException($"Bad language code: {src.Language}");

                Photo? photo = null;
                if (!string.IsNullOrEmpty(src.CoverPhoto))
                {
                    var photoResult = Photo.Create(src.CoverPhoto);
                    if (photoResult.IsFailed)
                        throw new AutoMapperMappingException($"Bad cover photo picture URL\n err: {photoResult.Errors}");
                    photo = photoResult.Value;
                }
                if (photo == null)
                    throw new AutoMapperMappingException("Somehow photo was null but error was not thrown");
                
                // map SocialMediaLinks if EF actually loaded them:
                var domainBookmarks = Enumerable.Empty<Bookmark>();
                if (src.Bookmarks != null && src.Bookmarks.Count != 0)
                {
                    domainBookmarks = ctx.Mapper
                        .Map<IEnumerable<Bookmark>>(src.Bookmarks);
                }

                var bookResult =  UserBook.Reconstitute(
                    id:            src.Id,
                    ownerId:       src.UserId,
                    generalBookId: src.BookId,
                    status:        src.Status,
                    state:         src.State,
                    language:      languageCodeResult.Value,
                    pageCount:     src.PageCount,
                    coverPhoto:    photo,
                    bookmarks:     domainBookmarks ?? []
                );

                if (bookResult.IsFailed)
                    throw new AutoMapperMappingException($"Was unable to map the UserBookEntity to UserBook during domain class creation \n err: {bookResult.Errors}");

                return bookResult.Value;
            });
        
        // Domain -> Entity Mapping
        CreateMap<UserBook, UserBookEntity>(MemberList.Source)
            .ForMember(dest => dest.Id,         opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId,     opt => opt.MapFrom(src => src.OwnerId))
            .ForMember(dest => dest.BookId,     opt => opt.MapFrom(src => src.GeneralBookId))
            .ForMember(dest => dest.Status,     opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.State,      opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.Language,   opt => opt.MapFrom(src => src.Language.Code))
            .ForMember(dest => dest.PageCount,  opt => opt.MapFrom(src => src.PageCount))
            .ForMember(dest => dest.CoverPhoto, opt => opt.MapFrom(src => src.CoverPhoto.Link))
            .ForMember(dest => dest.Bookmarks, opt => opt.Ignore());
    }
}
