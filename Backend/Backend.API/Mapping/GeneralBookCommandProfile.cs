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
        CreateMap<CreateGeneralBookRequest, CreateGeneralBookCommand>();
        CreateMap<UpdateGeneralBookRequest, UpdateGeneralBookCommand>();
        CreateMap<UpdateCoverRequest, UpdateGeneralBookCoverCommand>();
        CreateMap<ConfirmCoverRequest, ConfirmGBCoverCommand>();
        
        // Reviews
        CreateMap<ReviewRequest, CreateReviewCommand>();
        CreateMap<ReviewRequest, UpdateReviewCommand>();
        
        // Read model → DTO
        CreateMap<GeneralBookListItem, GeneralBookListItemResponse>()
            .ForMember(dest => dest.PrimaryGenre, 
                       opt => opt.MapFrom(src => src.PrimaryGenre.ToString()));
        
        CreateMap<GeneralBookDetailsReadModel, GeneralBookDetailsResponse>();
        CreateMap<ReviewReadModel, ReviewResponse>();
        
        // Pagination wrapper
        CreateMap<PaginatedResult<GeneralBookListItem>, PaginatedResponse<GeneralBookListItemResponse>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
}