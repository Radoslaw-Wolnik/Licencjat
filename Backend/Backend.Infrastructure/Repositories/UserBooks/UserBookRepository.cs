using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Auth;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Infrastructure.Repositories.UserBooks;

public interface IUserBookRepository :
    ICoreUserBookRepository,
    IUserBookBookmarksRepository
{ }


public class UserBookRepository : IUserBookRepository
{
    private readonly ICoreUserBookRepository _core;
    private readonly IUserBookBookmarksRepository _bookmarks;

    public UserRepository(
      ICoreUserBookRepository core,
      IUserBookBookmarksRepository bookmarks
    ) {
      _core = core;
      _bookmarks = bookmarks;
    }

    // --- ICoreUserBookRepository --- 


    // --- IUserBookBookmarksRepository ---
    
}
