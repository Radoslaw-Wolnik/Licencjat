using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;

namespace Backend.Infrastructure.Repositories.Users;

public class WriteUserRepository : IWriteUserRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public WriteUserRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task AddAsync(User user)
    {
        var entity = _mapper.Map<UserEntity>(user);
        _db.Users.Add(entity);
        await _db.SaveChangesAsync();
        
        // user.SetId(entity.Id); 
    }

    public async Task UpdateAsync(User user)
    {
        var existing = await _db.Users.FindAsync(user.Id);
        _mapper.Map(user, existing);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid userId)
    {
        var existing = await _db.Users.FindAsync(userId);
        if (existing is null)
            throw new KeyNotFoundException($"User with Id = {userId} was not found.");

        _db.Users.Remove(existing);
        await _db.SaveChangesAsync();  
    }
}
