using AutoMapper;
using Backend.API.DTOs.UserBooks;
using Backend.API.DTOs.UserBooks.Responses;
using Backend.Application.Commands.UserBooks.Bookmarks;
using Backend.Infrastructure.Extensions;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Application.Querries.UserBooks;
using Backend.API.DTOs.Common;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/userbooks/{userBookId:guid}/bookmarks")]
public sealed class BookmarksController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public BookmarksController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid userBookId,
        [FromBody] CreateBookmarkRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<CreateBookmarkCommand>(request) // with
        {
            UserBookId = userBookId,
            Metadata = { ["UserId"] = userId } // For ownership validation
        };

        var result = await _sender.Send(command);

        return result.Match(
            bookmark => CreatedAtAction(
                nameof(Get), 
                new { userBookId, bookmarkId = bookmark.Id }, 
                _mapper.Map<BookmarkResponse>(bookmark)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("{bookmarkId:guid}")]
    public async Task<IActionResult> Update(
        Guid userBookId,
        Guid bookmarkId,
        [FromBody] UpdateBookmarkRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<UpdateBookmarkCommand>(request) // with 
        {
            BookmarkId = bookmarkId,
            Metadata = { ["UserId"] = userId }
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            bookmark => Ok(_mapper.Map<BookmarkResponse>(bookmark)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{bookmarkId:guid}")]
    public async Task<IActionResult> Delete(
        Guid userBookId,
        Guid bookmarkId)
    {
        var userId = User.GetUserId();
        var command = new DeleteBookmarkCommand(bookmarkId) 
        { 
            Metadata = { ["UserId"] = userId } 
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            () => NoContent(),
            errors => errors.ToProblemDetailsResult()
        );
    }
}