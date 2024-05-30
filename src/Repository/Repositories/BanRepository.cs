using Common.Parameters;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts.Contracts;
using Repository.RepositoryExtensions;

namespace Repository.Repositories;

public class BanRepository : RepositoryBase<Ban>, IBanRepository
{
    public BanRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }
    
    public Task<Ban?> GetBanById(uint banId, bool trackChanges, CancellationToken cancellationToken) =>
        RepositoryContext.Bans
            .Where(b => b.Id == banId)
            .TrackChanges(trackChanges)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Ban?> GetBanByUserId(string userId, bool trackChanges, CancellationToken cancellationToken) =>
        RepositoryContext.Bans
            .Where(b => b.UserId == userId)
            .TrackChanges(trackChanges)
            .FirstOrDefaultAsync(cancellationToken);
    
    public async Task<IEnumerable<Ban>> GetBans(RequestParameters requestParameters, CancellationToken cancellationToken) =>
        await RepositoryContext.Bans
            .AsNoTracking()
            .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
            .Take(requestParameters.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

    public async Task<IEnumerable<Ban>> GetExpiredBans(bool trackChanges, CancellationToken cancellationToken) =>
        await RepositoryContext.Bans
            .Where(b => b.BanEndDate < DateTime.Now)
            .TrackChanges(trackChanges)
            .ToListAsync(cancellationToken: cancellationToken);
}