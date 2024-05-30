using Common.Parameters;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts.Contracts;
using Repository.RepositoryExtensions;

namespace Repository.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

    public Task<User?> GetUserById(string userId, bool trackChanges, CancellationToken cancellationToken) =>
        RepositoryContext.Users
            .Where(u => u.Id == userId)
            .TrackChanges(trackChanges)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

    public Task<User?> GetUserByEmail(string email, bool trackChanges, CancellationToken cancellationToken) =>
        RepositoryContext.Users
            .Where(u => u.Email == email)
            .TrackChanges(trackChanges)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    
    public Task<User?> GetUserByUserName(string userName, bool b, CancellationToken cancellationToken) =>
        RepositoryContext.Users
            .Where(u => u.UserName == userName)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

    public async Task<IEnumerable<User>> GetAllUsers(CancellationToken cancellationToken, RequestParameters requestParameters) =>
        await RepositoryContext.Users
            .AsNoTracking()
            .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
            .Take(requestParameters.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

    public void DetachUser(string userId)
    {
        var user = RepositoryContext.Users.Local.FirstOrDefault(u => u.Id == userId);
        if (user != null)
            RepositoryContext.Entry(user).State = EntityState.Detached;
    }
}