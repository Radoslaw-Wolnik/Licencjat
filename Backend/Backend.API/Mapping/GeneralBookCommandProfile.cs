using AutoMapper;
using Backend.API.DTOs.Common;
using Backend.API.DTOs.GeneralBooks;
using Backend.API.DTOs.GeneralBooks.Responses;
using Backend.Application.Commands.GeneralBooks.Core;
using Backend.Application.Commands.GeneralBooks.Reviews;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Common;

namespace Backend.API.Mapping;

public sealed class GeneralBookCommandProfile : Profile
{
    public GeneralBookCommandProfile()
    {
        // General Books
        CreateMap<CreateGeneralBookRequest, CreateGeneralBookCommand>()
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Author))
            .ForCtorParam("Published", opt => opt.MapFrom(src => src.Published))
            .ForCtorParam("OryginalLanguage", opt => opt.MapFrom(src => src.OriginalLanguage))
            .ForCtorParam("CoverFileName", opt => opt.MapFrom(src => src.CoverFileName));

        CreateMap<UpdateGeneralBookRequest, UpdateGeneralBookCommand>()
            // .ForCtorParam("BookId", opt => opt.MapFrom(src => src.BookId)) // in command
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Author))
            .ForCtorParam("Published", opt => opt.MapFrom(src => src.Published))
            .ForCtorParam("OryginalLanguage", opt => opt.MapFrom(src => src.OriginalLanguage))
            .ForCtorParam("NewBookGenres", opt => opt.MapFrom(src => src.NewBookGenres));


        CreateMap<UpdateCoverRequest, UpdateGeneralBookCoverCommand>()
            // .ForCtorParam("BookId", opt => opt.MapFrom(src => src.BookId)) // addtionally in command
            .ForCtorParam("CoverFileName", opt => opt.MapFrom(src => src.CoverFileName));


        CreateMap<ConfirmCoverRequest, ConfirmGBCoverCommand>()
            // .ForCtorParam("BookId", opt => opt.MapFrom(src => src.BookId)) // additional to the request
            .ForCtorParam("ImageObjectKey", opt => opt.MapFrom(src => src.ImageObjectKey));


        // Reviews
        CreateMap<ReviewRequest, CreateReviewCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.UserId))
            // .ForCtorParam("BookId", opt => opt.MapFrom(src => src.BookId)) // additional ot the request
            .ForCtorParam("Rating", opt => opt.MapFrom(src => src.Rating))
            .ForCtorParam("Comment", opt => opt.MapFrom(src => src.Comment));

        CreateMap<ReviewRequest, UpdateReviewCommand>()
            // .ForCtorParam("ReviewId", opt => opt.MapFrom(src => src.ReviewId)) additional to the request
            .ForCtorParam("Rating", opt => opt.MapFrom(src => src.Rating))
            .ForCtorParam("Comment", opt => opt.MapFrom(src => src.Comment));

        // Read model → DTO
        CreateMap<GeneralBookListItem, GeneralBookListItemResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Author))
            .ForCtorParam("CoverUrl", opt => opt.MapFrom(src => src.CoverUrl))
            .ForCtorParam("RatingAvg", opt => opt.MapFrom(src => src.RatingAvg))
            .ForCtorParam("PrimaryGenre",
                       opt => opt.MapFrom(src => src.PrimaryGenre.ToString()))
            .ForCtorParam("PublicationDate", opt => opt.MapFrom(src => src.PublicationDate));

        CreateMap<BookCoverItemReadModel, BookCoverItemResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("CoverUrl", opt => opt.MapFrom(src => src.CoverUrl));
        
        CreateMap<GeneralBookDetailsReadModel, GeneralBookDetailsResponse>();
        CreateMap<ReviewReadModel, ReviewResponse>();

        // Pagination wrapper
        CreateMap<PaginatedResult<GeneralBookListItem>, PaginatedResponse<GeneralBookListItemResponse>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
}