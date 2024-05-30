using Common.Parameters;
using Domain.Entities;

namespace Repository.Contracts.Contracts;

public interface IUserRepository : IRepositoryBase<User>
{
    Task<User?> GetUserById(string userId, bool trackChanges, CancellationToken cancellationToken);
    
    Task<User?> GetUserByEmail(string email, bool trackChanges, CancellationToken cancellationToken);
    
    Task<User?> GetUserByUserName(string userName, bool b, CancellationToken cancellationToken);
    
    Task<IEnumerable<User>> GetAllUsers(CancellationToken cancellationToken, RequestParameters requestParameters);
}