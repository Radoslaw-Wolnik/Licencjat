using Backend.API.DTOs.Users;
using Backend.Application.Commands.Users.Wishlist;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Infrastructure.Extensions;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users/wishlist")]
public sealed class WishlistController : ControllerBase
{
    private readonly ISender _sender;
    
    public WishlistController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist(
        [FromBody] WishlistBookRequest request)
    {
        var userId = User.GetUserId();
        var command = new AddWishlistBookCommand(userId, request.BookId);
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => CreatedAtAction(
                nameof(Get), 
                new { bookId = request.BookId }, 
                new { BookId = request.BookId }),
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
}