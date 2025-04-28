using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Auth;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Infrastructure.Repositories.GeneralBooks;

public interface IGeneralBookRepository :
    ICoreGeneralBookRepository,
    IGeneralBookReviewRepository
{ }


public class GeneralBookRepository : IGeneralBookRepository
{
    private readonly ICoreUserBookRepository _core;
    private readonly IGeneralBookReviewRepository _reviews;

    public GeneralBookRepository(
      ICoreUserBookRepository core,
      IGeneralBookReviewRepository reviews
    ) {
      _core = core;
      _reviews = reviews;
    }

    // --- ICoreGeneralBookRepository --- 


    // --- IGeneralBookReviewRepository ---
    
}
