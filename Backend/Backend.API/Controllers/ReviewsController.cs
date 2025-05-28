using AutoMapper;
using Backend.API.DTOs.Common;
using Backend.API.DTOs.GeneralBooks;
using Backend.API.DTOs.GeneralBooks.Responses;
using Backend.API.Extensions;
using Backend.Application.Commands.GeneralBooks.Reviews;
using Backend.Application.Querries.GeneralBooks;
using Backend.Domain.Enums.SortBy;
using Backend.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/generalbooks/{bookId:guid}/[controller]")]
public sealed class ReviewsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public ReviewsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> List(
        Guid bookId,
        [FromQuery] SortReviewsBy sortBy = SortReviewsBy.Date,
        [FromQuery] bool descending = false,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 20)
    {
        var query = new ListReviewsQuerry(
            bookId,
            sortBy,
            descending,
            offset,
            limit
        );
        
        var result = await _sender.Send(query);
        
        return result.Match(
            paginated => Ok(_mapper.Map<PaginatedResponse<ReviewResponse>>(paginated)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpGet("{reviewId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(Guid bookId, Guid reviewId)
    {
        var query = new GetReviewByIdQuery(bookId, reviewId);
        var result = await _sender.Send(query);
        
        return result.Match(
            review => Ok(_mapper.Map<ReviewResponse>(review)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        Guid bookId, 
        [FromBody] ReviewRequest request)
    {
        var userId = User.GetUserId(); // Extension method to get user ID
        var command = _mapper.Map<CreateReviewCommand>(request) with 
        { 
            UserId = userId, 
            BookId = bookId 
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: reviewId => CreatedAtAction(
                nameof(Get), 
                new { bookId, reviewId }, 
                new { ReviewId = reviewId }),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("{reviewId:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(
        Guid bookId, 
        Guid reviewId, 
        [FromBody] ReviewRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<UpdateReviewCommand>(request) with 
        { 
            ReviewId = reviewId 
        };
        
        // Add user ID to command metadata for handler validation
        command.Metadata.Add("UserId", userId);
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{reviewId:guid}")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Delete(Guid bookId, Guid reviewId)
    {
        var command = new DeleteReviewCommand(reviewId);
        var result = await _sender.Send(command);

        // return result.ToActionResult();
        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }
}