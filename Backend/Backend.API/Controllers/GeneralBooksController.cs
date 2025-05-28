using AutoMapper;
using Backend.API.DTOs.Common;
using Backend.API.DTOs.GeneralBooks;
using Backend.API.DTOs.GeneralBooks.Responses;
using Backend.API.Extensions;
using Backend.Application.Commands.GeneralBooks.Core;
using Backend.Application.Querries.GeneralBooks;
using Backend.Domain.Enums;
using Backend.Domain.Enums.SortBy;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GeneralBooksController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public GeneralBooksController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] DTOs.GeneralBooks.CreateGeneralBookRequest request)
    {
        var command = _mapper.Map<CreateGeneralBookCommand>(request);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: tuple => CreatedAtAction(
                nameof(Get),
                new { id = tuple.Item1 },
                _mapper.Map<CreateGeneralBookResponse>(tuple)),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateGeneralBookRequest request)
    {
        var command = _mapper.Map<UpdateGeneralBookCommand>(request) with { BookId = id };
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: book => Ok(book),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPatch("{id:guid}/cover")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCover(
        Guid id,
        [FromBody] UpdateCoverRequest request)
    {
        var command = _mapper.Map<UpdateGeneralBookCoverCommand>(request) with { BookId = id };
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: newKey => Ok(new { ImageObjectKey = newKey }),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost("{id:guid}/cover/confirm")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmCover(
        Guid id,
        [FromBody] ConfirmCoverRequest request)
    {
        var command = _mapper.Map<ConfirmGBCoverCommand>(request) with { BookId = id };
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromQuery] string photoKey)
    {
        var command = new DeleteGeneralBookCommand(id, photoKey);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }
}