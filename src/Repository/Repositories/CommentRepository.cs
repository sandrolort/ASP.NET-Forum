using Common.Parameters;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts.Contracts;
using Repository.RepositoryExtensions;

namespace Repository.Repositories;

public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
{
    public CommentRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

    public Task<Comment?> GetCommentById(uint commentId, bool trackChanges, bool includeUser,
        CancellationToken cancellationToken) =>
        RepositoryContext.Comments
            .Where(c => c.Id == commentId)
            .ConditionalInclude(c => c.Author, includeUser)
            .TrackChanges(trackChanges)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Comment>> GetCommentsByTopicId(uint topicId, bool includeUser, CancellationToken cancellationToken,
        RequestParameters requestParameters) =>
        await RepositoryContext.Comments
            .Where(c=>c.TopicId == topicId)
            .ConditionalInclude(c=>c.Author,includeUser)
            .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
            .Take(requestParameters.PageSize)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Comment>> GetCommentsByUserId(string userId, CancellationToken cancellationToken, 
        RequestParameters requestParameters) =>
        await RepositoryContext.Comments
            .Where(c => c.AuthorId == userId)
            .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
            .Take(requestParameters.PageSize)
            .ToListAsync(cancellationToken);

    public Task<int> GetCommentsCountByTopicId(uint topicId, CancellationToken cancellationToken) =>
        RepositoryContext.Comments
            .CountAsync(c => c.TopicId == topicId, cancellationToken);

    public Task<int> GetCommentsCountByUserId(string userId, CancellationToken cancellationToken) =>
        RepositoryContext.Comments
            .CountAsync(c => c.AuthorId == userId, cancellationToken);

    public Task DeleteCommentsByUserId(string userId) =>
        RepositoryContext.Comments
            .Where(c => c.AuthorId == userId)
            .ForEachAsync(c => RepositoryContext.Comments.Remove(c));
}