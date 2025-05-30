using Backend.API.DTOs.Users;
using Backend.Application.Commands.Users.Wishlist;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Infrastructure.Extensions;
using Backend.API.DTOs.Common;
using Backend.Domain.Common;
using Backend.Application.ReadModels.Common;
using Backend.Application.Querries.Users.Collections;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users/wishlist")]
public sealed class WishlistController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public WishlistController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist(
        [FromBody] WishlistBookRequest request)
    {
        var userId = User.GetUserId();
        var command = new AddWishlistBookCommand(userId, request.BookId);

        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{bookId:guid}")]
    public async Task<IActionResult> RemoveFromWishlist(Guid bookId)
    {
        var userId = User.GetUserId();
        var command = new RemoveWishlistBookCommand(userId, bookId);

        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlist(
       [FromQuery] string? title,
       [FromQuery] bool descending = false,
       [FromQuery] int offset = 0,
       [FromQuery][Range(1, 100)] int limit = 20)
    {
        var userId = User.GetUserId();
        var query = new ListWishlistQuery(
            userId,
            title,
            descending,
            offset,
            limit);

        var result = await _sender.Send(query);

        return result.Match(
            paginated => Ok(_mapper.Map<PaginatedResponse<BookCoverItemResponse>>(paginated)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    
}