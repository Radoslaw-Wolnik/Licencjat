using AutoMapper;
using Backend.API.DTOs.Common;
using Backend.API.DTOs.UserBooks;
using Backend.API.DTOs.UserBooks.Responses;
using Backend.Application.Commands.UserBooks.Core;
using Backend.Infrastructure.Extensions;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Application.Querries.UserBooks;
using Backend.Domain.Enums.SortBy;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/userbooks")]
public sealed class UserBooksController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public UserBooksController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("{id:guid}")]
    [Authorize] // Or [Authenticate] not sure
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetUserBookByIdQuerry(id);
        var result = await _sender.Send(query);
        
        return result.Match(
            userBook => Ok(_mapper.Map<UserBookDetailsResponse>(userBook)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserLibrary(
        [FromQuery] string? nameFilter,
        [FromQuery] string? authorFilter,
        [FromQuery] SortUserBookBy sortBy = SortUserBookBy.Title,
        [FromQuery] bool descending = false,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 20)
    {
        var userId = User.GetUserId();
        var query = new ListLibraryQuerry(
            UserId: userId,
            NameFilter: nameFilter,
            AuthorFilter: authorFilter,
            SortBy: sortBy,
            Descending: descending,
            Offset: offset,
            Limit: limit
        );
        
        var result = await _sender.Send(query);
        
        return result.Match(
            paginated => Ok(_mapper.Map<PaginatedResponse<UserLibraryItemResponse>>(paginated)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ListByGeneralBook(
        [FromQuery] Guid generalBookId,
        [FromQuery] SortUserBookBy sortBy = SortUserBookBy.Title,
        [FromQuery] bool descending = false,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 20)
    {
        var query = new ListUserBooksQuerry(
            GeneralBookId: generalBookId,
            SortBy: sortBy,
            Descending: descending,
            Offset: offset,
            Limit: limit
        );
        
        var result = await _sender.Send(query);
        
        return result.Match(
            paginated => Ok(_mapper.Map<PaginatedResponse<UserBookListItemResponse>>(paginated)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpGet("{id:guid}/profile")]
    [Authorize]  // Only owner can access
    public async Task<IActionResult> GetOwnBookProfile(Guid id)
    {
        var userId = User.GetUserId();
        var query = new GetUserOwnBookProfileByIdQuerry(id);
        var result = await _sender.Send(query);
        
        // Add ownership validation in handler or here
        return result.Match(
            profile => Ok(_mapper.Map<UserOwnBookProfileResponse>(profile)),
            errors => errors.ToProblemDetailsResult()
        );
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserBookRequest request)
    {
        var userId = User.GetUserId(); // Extension method to get user ID
        var command = _mapper.Map<CreateUserBookCommand>(request) with { UserId = userId };
        var result = await _sender.Send(command);

        return result.Match(
            tuple => CreatedAtAction(
                nameof(Get), 
                new { id = tuple.Item1 }, 
                _mapper.Map<CreateUserBookResponse>(tuple)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, 
        [FromBody] UpdateUserBookRequest request)
    {
        var command = _mapper.Map<UpdateUserBookCommand>(request) with { UserBookId = id };
        var result = await _sender.Send(command);

        return result.Match(
            userBook => Ok(_mapper.Map<UserBookResponse>(userBook)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPatch("{id:guid}/cover")]
    public async Task<IActionResult> UpdateCover(
        Guid id, 
        [FromBody] UpdateCoverRequest request)
    {
        var command = _mapper.Map<UpdateUserBookCoverCommand>(request) with { UserBookId = id };
        var result = await _sender.Send(command);

        return result.Match(
            newKey => Ok(new { ImageObjectKey = newKey }),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost("{id:guid}/cover/confirm")]
    public async Task<IActionResult> ConfirmCover(
        Guid id, 
        [FromBody] ConfirmCoverRequest request)
    {
        var command = _mapper.Map<ConfirmUBCoverCommand>(request) with { UserBookId = id };
        var result = await _sender.Send(command);

        return result.Match(
            () => NoContent(),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteUserBookCommand(id);
        var result = await _sender.Send(command);

        return result.Match(
            () => NoContent(),
            errors => errors.ToProblemDetailsResult()
        );
    }
}