using Common.Parameters;
using Domain.Entities;

namespace Repository.Contracts.Contracts;

public interface ICommentRepository : IRepositoryBase<Comment>
{
    Task<Comment?> GetCommentById(uint commentId,
        bool trackChanges,
        bool includeUser,
        CancellationToken cancellationToken);
    
    Task<IEnumerable<Comment>> GetCommentsByTopicId(uint topicId,
        bool includeUser,
        CancellationToken cancellationToken,
        RequestParameters requestParameters);

    Task<IEnumerable<Comment>> GetCommentsByUserId(string userId,
        CancellationToken cancellationToken,
        RequestParameters requestParameters);
    
    Task<int> GetCommentsCountByTopicId(uint topicId, CancellationToken cancellationToken);
    
    Task<int> GetCommentsCountByUserId(string userId, CancellationToken cancellationToken);
    
    Task DeleteCommentsByUserId(string userId);
}