using Common.Enums;
using Common.Parameters;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts.Contracts;
using Repository.RepositoryExtensions;

namespace Repository.Repositories;

public class TopicRepository : RepositoryBase<Topic>, ITopicRepository
{
    public TopicRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

    public Task<Topic?> GetTopicById(uint topicId, bool trackChanges, bool includeUser, CancellationToken cancellationToken) =>
        RepositoryContext.Topics
            .Where(t => t.Id == topicId)
            .ConditionalInclude(t => t.Author, includeUser)
            .TrackChanges(trackChanges)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    
    public async Task<IEnumerable<Topic>> GetAllTopics(bool includeUser, CancellationToken cancellationToken, TopicParameters requestParameters) =>
        await RepositoryContext.Topics
            .FilterConfirmed()
            .ConditionalInclude(t => t.Author, includeUser)
            .Search(requestParameters.Search)
            .Sort(requestParameters.OrderBy)
            .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
            .Take(requestParameters.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);
    
    public async Task<IEnumerable<Topic>> GetAllTopics(CancellationToken cancellationToken) =>
        await RepositoryContext.Topics
            .FilterConfirmed()
            .ToListAsync(cancellationToken: cancellationToken);

    public async Task<IEnumerable<Topic>> GetTopicsByUserId(string userId, CancellationToken cancellationToken, RequestParameters requestParameters) =>
        await RepositoryContext.Topics
            .Where(t => t.AuthorId == userId)
            .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
            .Take(requestParameters.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

    public async Task<IEnumerable<Topic>> GetArchivableTopics(int archiveAfterDays, CancellationToken cancellationToken) =>
        await RepositoryContext.Topics
            .FilterByAge(archiveAfterDays)
            .ToListAsync(cancellationToken: cancellationToken);

    public async Task<IEnumerable<Topic>> GetUnconfirmedTopics(CancellationToken cancellationToken, RequestParameters parameters, bool includeAuthor) =>
        await RepositoryContext.Topics
            .Where(t=>t.State == State.Pending)
            .ConditionalInclude(t => t.Author, includeAuthor)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

    public Task DeleteTopicsByUserId(string userId) =>
        RepositoryContext.Topics
            .Where(t => t.AuthorId == userId)
            .ForEachAsync(t => RepositoryContext.Topics.Remove(t));
}