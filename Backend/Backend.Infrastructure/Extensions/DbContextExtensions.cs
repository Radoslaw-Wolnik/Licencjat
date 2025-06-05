using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<Result> SaveChangesWithResultAsync(
                this DbContext ctx,
                CancellationToken ct,
                string errorMessage = "Database error"
        ) {
            try
            {
                await ctx.SaveChangesAsync(ct);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                // return Result.Fail(new Error(errorMessage).CausedBy(ex));
                return Result.Fail(DomainErrorFactory.StorageError(errorMessage + $"\n{ex}")); 
            }
        }
    }
}
