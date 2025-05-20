using Backend.Application.Interfaces.Repositories;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Domain.Common;

namespace Backend.Application.Commands.UserBooks.Core;
public class UpdateUserBookCommandHandler
    : IRequestHandler<UpdateUserBookCommand, Result<UserBook>>
{
    private readonly IWriteUserBookRepository _bookRepo;
    private readonly IUserBookReadService _bookRead;

    public UpdateUserBookCommandHandler(
        IWriteUserBookRepository bookRepo,
        IUserBookReadService bookRead)
    {
        _bookRepo = bookRepo;
        _bookRead = bookRead;
    }

    public async Task<Result<UserBook>> Handle(
        UpdateUserBookCommand request,
        CancellationToken cancellationToken)
    {
        // get book
        var book = await _bookRead.GetByIdAsync(request.UserBookId, cancellationToken);
        
        if (book == null)
            return Result.Fail(DomainErrorFactory.NotFound("UserBook", request.UserBookId));

        if (request.State != null)
            book.UpdateState((Domain.Enums.BookState)request.State);
        if (request.Status != null)
            book.UpdateStatus((Domain.Enums.BookStatus)request.Status);
        
        // and persistance save
        var saveResult = await _bookRepo.UpdateScalarsAsync(book, cancellationToken);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);

        return Result.Ok(book);
    }
}