using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Domain.Errors;

namespace Backend.Application.Commands.GeneralBooks.Core;
public class DeleteGeneralBookCommandHandler
    : IRequestHandler<DeleteGeneralBookCommand, Result>
{
    private readonly IWriteGeneralBookRepository _bookRepo;
    private readonly IImageStorageService _imageStorage;
    private readonly IUserContext _userContext;


    public DeleteGeneralBookCommandHandler(
        IWriteGeneralBookRepository bookRepo,
        IImageStorageService storage,
        IUserContext userContext)
    {
        _bookRepo = bookRepo;
        _imageStorage = storage;
        _userContext = userContext;
    }

    public async Task<Result> Handle(
        DeleteGeneralBookCommand request,
        CancellationToken cancellationToken)
    {
        // Security: Validate user context
        if (!_userContext.IsAuthenticated)
            return Result.Fail(DomainErrorFactory.Unauthorized("GeneralBook.Create", "user is not logged in"));

        // check if user has admin privileges
        if (!_userContext.IsInRole("Admin"))
            return Result.Fail(DomainErrorFactory.Forbidden("GeneralBook.Create", "Admin role required"));
            
        var Id = request.GeneralBookId;

        // ask the storage to delete the book
        var deleteResult = await _bookRepo.DeleteAsync(Id, cancellationToken);
        if (deleteResult.IsFailed)
            return Result.Fail(deleteResult.Errors);
        
        // ask the image storage service to delete thse photos
        await _imageStorage.DeleteAsync(request.PhotoKey, cancellationToken);

        return Result.Ok();
    }
}
