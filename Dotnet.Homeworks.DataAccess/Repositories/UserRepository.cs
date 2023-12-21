using Dotnet.Homeworks.Data.DatabaseContext;
using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Homeworks.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext userRepository)
    {
        _dbContext = userRepository;
    }

    public Task<IQueryable<User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IQueryable<User>>(_dbContext.Users);
    }

    public async Task<User?> GetUserByGuidAsync(Guid guid, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == guid, cancellationToken);
    }

    public async Task DeleteUserByGuidAsync(Guid guid, CancellationToken cancellationToken)
    {
        await _dbContext.Users
            .Where(u => u.Id == guid)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users
            .ExecuteUpdateAsync(u => u
                .SetProperty(u => u.Name, user.Name)
                .SetProperty(u => u.Email, user.Email), cancellationToken);
    }

    public async Task<Guid> InsertUserAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        return user.Id;
    }
}