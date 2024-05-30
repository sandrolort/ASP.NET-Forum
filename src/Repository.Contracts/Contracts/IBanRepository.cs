using Common.Parameters;
using Domain.Entities;

namespace Repository.Contracts.Contracts;

public interface IBanRepository : IRepositoryBase<Ban>
{
    Task<Ban?> GetBanById(uint banId, bool trackChanges, CancellationToken cancellationToken);
    
    Task<Ban?> GetBanByUserId(string userId, bool trackChanges, CancellationToken cancellationToken);
    
    Task<IEnumerable<Ban>> GetBans(RequestParameters requestParameters, CancellationToken cancellationToken);
    Task<IEnumerable<Ban>> GetExpiredBans(bool trackChanges, CancellationToken cancellationToken);
}