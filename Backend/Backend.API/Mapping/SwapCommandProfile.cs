using AutoMapper;
using Backend.API.DTOs.Common;
using Backend.API.DTOs.Swaps;
using Backend.API.DTOs.Swaps.Responses;

using Backend.Application.Commands.Swaps.Core;
using Backend.Application.Commands.Swaps.Feedbacks;
using Backend.Application.Commands.Swaps.Issues;
using Backend.Application.Commands.Swaps.Meetups;
using Backend.Application.ReadModels.Swaps;
using Backend.Domain.Common;
using Backend.Domain.Entities;

namespace Backend.API.Mapping;

public class SwapCommandProfile : Profile
{
    public SwapCommandProfile()
    {
        // Core Swaps
        CreateMap<CreateSwapRequest, CreateSwapCommand>();
        CreateMap<AcceptSwapRequest, AcceptSwapCommand>();
        CreateMap<UpdateSwapRequest, UpdateSwapCommand>();

        // Feedbacks
        CreateMap<AddFeedbackRequest, AddFeedbackCommand>();

        // Issues
        CreateMap<AddIssueRequest, AddIssueCommand>();
        CreateMap<RemoveIssueRequest, RemoveIssueCommand>();

        // Meetups
        CreateMap<AddMeetupRequest, AddMeetupCommand>();
        CreateMap<UpdateMeetupRequest, UpdateMeetupCommand>();

        // Responses
        CreateMap<Swap, SwapResponse>();
        CreateMap<Feedback, FeedbackResponse>();
        CreateMap<Issue, IssueResponse>();
        CreateMap<Meetup, MeetupResponse>();

        // Read models → DTOs
        CreateMap<SwapDetailsReadModel, SwapDetailsResponse>()
            .ForMember(dest => dest.LastStatus, opt => opt.MapFrom(src => src.LastStatus.ToString()));

        CreateMap<SubSwapReadModel, SubSwapResponse>();
        CreateMap<SwapListItem, SwapListItemResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<TimelineUpdateReadModel, TimelineUpdateResponse>();
        CreateMap<MeetupReadModel, MeetupResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<IssueReadModel, IssueResponse>();
        CreateMap<FeedbackReadModel, FeedbackResponse>();

        // Pagination
        CreateMap<PaginatedResult<SwapListItem>, PaginatedResponse<SwapListItemResponse>>();
        CreateMap<PaginatedResult<TimelineUpdateReadModel>, PaginatedResponse<TimelineUpdateResponse>>();
        
    }
}