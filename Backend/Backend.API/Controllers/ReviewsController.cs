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