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
        CreateMap<CreateSwapRequest, CreateSwapCommand>()
            // .ForCtorParam("UserRequestingId", opt => opt.MapFrom(src => src.UserRequestingId))
            .ForCtorParam("UserAcceptingId", opt => opt.MapFrom(src => src.UserAcceptingId))
            .ForCtorParam("RequestedBookId", opt => opt.MapFrom(src => src.RequestedBookId));

        CreateMap<AcceptSwapRequest, AcceptSwapCommand>()
            // .ForCtorParam("UserAcceptingId", opt => opt.MapFrom(src => src.UserId))
            .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SwapId))
            .ForCtorParam("RequestedBookId", opt => opt.MapFrom(src => src.RequestedBookId));

        CreateMap<UpdateSwapRequest, UpdateSwapCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.UserId))
            .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SwapId))
            .ForCtorParam("PageAt", opt => opt.MapFrom(src => src.PageAt));

        // Feedbacks
        CreateMap<AddFeedbackRequest, AddFeedbackCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.UserId))
            .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SwapId))
            .ForCtorParam("Stars", opt => opt.MapFrom(src => src.Stars))
            .ForCtorParam("Recomended", opt => opt.MapFrom(src => src.Recommend))
            .ForCtorParam("Lenght", opt => opt.MapFrom(src => src.Length))
            .ForCtorParam("Condition", opt => opt.MapFrom(src => src.Condition))
            .ForCtorParam("Communication", opt => opt.MapFrom(src => src.Communication));

        // Issues
        CreateMap<AddIssueRequest, AddIssueCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.UserId))
            .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SwapId))
            .ForCtorParam("Description", opt => opt.MapFrom(src => src.Description));

        CreateMap<RemoveIssueRequest, RemoveIssueCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.UserId))
            .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SwapId))
            .ForCtorParam("IssueId", opt => opt.MapFrom(src => src.IssueId))
            .ForCtorParam("ResolutionDetails", opt => opt.MapFrom(src => src.ResolutionDetails));

        // Meetups
        CreateMap<AddMeetupRequest, AddMeetupCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.UserId))
            // .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SwapId))
            .ForCtorParam("Latitude", opt => opt.MapFrom(src => src.Latitude))
            .ForCtorParam("Longitude", opt => opt.MapFrom(src => src.Longitude));

        CreateMap<UpdateMeetupRequest, UpdateMeetupCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.UserId))
            .ForCtorParam("MeetupId", opt => opt.MapFrom(src => src.MeetupId))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("Latitude", opt => opt.MapFrom(src => src.Latitude))
            .ForCtorParam("Longitude", opt => opt.MapFrom(src => src.Longitude));

        // Responses
        CreateMap<Swap, SwapResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("UserRequestingId", opt => opt.MapFrom(src => src.SubSwapRequesting.UserId))
            .ForCtorParam("UserAcceptingId", opt => opt.MapFrom(src => src.SubSwapAccepting.UserId))
            .ForCtorParam("RequestedBookId", opt => opt.MapFrom(src => src.SubSwapRequesting.UserBookReading!.Id))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt))
            ;// ignore rest;

        CreateMap<Feedback, FeedbackResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SubSwapId)) // not relly but its hard to get the swapId back - shoudl fix this
            .ForCtorParam("Stars", opt => opt.MapFrom(src => src.Stars))
            .ForCtorParam("Recomended", opt => opt.MapFrom(src => src.Recommend))
            .ForCtorParam("Lenght", opt => opt.MapFrom(src => src.Length))
            .ForCtorParam("Condition", opt => opt.MapFrom(src => src.Condition))
            .ForCtorParam("Communication", opt => opt.MapFrom(src => src.Communication))
            ; // ignore rest;

        CreateMap<Issue, IssueResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SubSwapId)) // not swapId
            .ForCtorParam("Description", opt => opt.MapFrom(src => src.Description))
            // .ForCtorParam("ReportedAt", opt => opt.MapFrom(src => src.CreatedAt)) // missing
            .ForCtorParam("ReportedAt", opt => opt.MapFrom(DateTime.Now.ToString()))
            .ForCtorParam("ResolutionDetails", opt => opt.MapFrom(src => src.Description)) // im missing resolution and resolved - bool
            ; // ignore rest ;

        CreateMap<Meetup, MeetupResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SwapId))
            .ForCtorParam("Latitude", opt => opt.MapFrom(src => src.Location.Latitude))
            .ForCtorParam("Longitude", opt => opt.MapFrom(src => src.Location.Longitude))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status));

        // Read models → DTOs
        CreateMap<SwapDetailsReadModel, SwapDetailsResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("MySubSwap", opt => opt.MapFrom(src => src.MySubSwap))
            .ForCtorParam("TheirSubSwap", opt => opt.MapFrom(src => src.TheirSubSwap))
            .ForCtorParam("SocialMediaLinks", opt => opt.MapFrom(src => src.SocialMediaLinks.ToList()))
            .ForCtorParam("LastStatus", opt => opt.MapFrom(src => src.LastStatus.ToString()))
            .ForCtorParam("Updates", opt => opt.MapFrom(src => src.Updates.ToList()))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<SubSwapReadModel, SubSwapResponse>()
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("CoverPhotoUrl", opt => opt.MapFrom(src => src.CoverPhotoUrl))
            .ForCtorParam("PageCount", opt => opt.MapFrom(src => src.PageCount))
            .ForCtorParam("UserName", opt => opt.MapFrom(src => src.UserName))
            .ForCtorParam("ProfilePictureUrl", opt => opt.MapFrom(src => src.ProfilePictureUrl));

        CreateMap<SwapListItem, SwapListItemResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("MyBookCoverUrl", opt => opt.MapFrom(src => src.MyBookCoverUrl))
            .ForCtorParam("TheirBookCoverUrl", opt => opt.MapFrom(src => src.TheirBookCoverUrl))
            .ForCtorParam("User", opt => opt.MapFrom(src => src.User))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<TimelineUpdateReadModel, TimelineUpdateResponse>()
            .ForCtorParam("Comment", opt => opt.MapFrom(src => src.Comment))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt))
            .ForCtorParam("UserName", opt => opt.MapFrom(src => src.UserName))
            .ForCtorParam("ProfilePictureUrl", opt => opt.MapFrom(src => src.ProfilePictureUrl));

        CreateMap<MeetupReadModel, MeetupResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SwapId))
            .ForCtorParam("Latitude", opt => opt.MapFrom(src => src.Latitude))
            .ForCtorParam("Longitude", opt => opt.MapFrom(src => src.Longitude))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<IssueReadModel, IssueResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SwapId))
            .ForCtorParam("Description", opt => opt.MapFrom(src => src.Description))
            .ForCtorParam("ReportedAt", opt => opt.MapFrom(DateTime.Now.ToString())) // CreatedAt
            .ForCtorParam("ResolutionDetails", opt => opt.MapFrom(src => src.Description)); // missing in issue model

        CreateMap<FeedbackReadModel, FeedbackResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("SwapId", opt => opt.MapFrom(src => src.SwapId))
            .ForCtorParam("Stars", opt => opt.MapFrom(src => src.Stars))
            .ForCtorParam("Recommend", opt => opt.MapFrom(src => src.Recommend))
            .ForCtorParam("Length", opt => opt.MapFrom(src => src.Length))
            .ForCtorParam("Condition", opt => opt.MapFrom(src => src.Condition))
            .ForCtorParam("Communication", opt => opt.MapFrom(src => src.Communication));

        // Pagination
        CreateMap<PaginatedResult<SwapListItem>, PaginatedResponse<SwapListItemResponse>>();
        CreateMap<PaginatedResult<TimelineUpdateReadModel>, PaginatedResponse<TimelineUpdateResponse>>();
        
    }
}