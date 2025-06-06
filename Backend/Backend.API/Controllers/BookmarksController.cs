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

    [HttpGet]
    public async Task<IActionResult> List(
        Guid userBookId,
        [FromQuery] bool descending = false,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 20)
    {
        var userId = User.GetUserId();
        var query = new ListBookmarksQuerry(
            UserBookId: userBookId,
            Descending: descending,
            Offset: offset,
            Limit: limit
        );
        
        // Add ownership validation in handler
        var result = await _sender.Send(query);
        
        return result.Match(
            paginated => Ok(_mapper.Map<PaginatedResponse<BookmarkResponse>>(paginated)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpGet("{bookmarkId:guid}")]
    public async Task<IActionResult> Get(
        Guid bookmarkId)
    {
        var query = new GetBookmarkByIdQuery(bookmarkId);
        var result = await _sender.Send(query);
        
        // Add ownership validation
        return result.Match(
            bookmark => Ok(_mapper.Map<BookmarkResponse>(bookmark)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid userBookId,
        [FromBody] CreateBookmarkRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<CreateBookmarkCommand>(request) with
        {
            UserBookId = userBookId
        };

        var result = await _sender.Send(command);

        return result.Match(
            bookmarkId => CreatedAtAction(
                nameof(Get), 
                new { bookmarkId }),
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
        var command = _mapper.Map<UpdateBookmarkCommand>(request) with
        {
            BookmarkId = bookmarkId
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            bookmark => Ok(_mapper.Map<BookmarkResponse>(bookmark)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{bookmarkId:guid}")]
    public async Task<IActionResult> Delete(
        Guid bookmarkId)
    {
        var userId = User.GetUserId();
        var command = new DeleteBookmarkCommand(bookmarkId);
        
        var result = await _sender.Send(command);

        return result.Match(
            () => NoContent(),
            errors => errors.ToProblemDetailsResult()
        );
    }
}