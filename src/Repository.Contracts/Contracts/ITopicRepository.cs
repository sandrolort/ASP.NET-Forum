using Common.Parameters;
using Domain.Entities;

namespace Repository.Contracts.Contracts;

public interface ITopicRepository : IRepositoryBase<Topic>
{
    Task<Topic?> GetTopicById(uint topicId, bool trackChanges, bool includeUser, CancellationToken cancellationToken);
    
    Task<IEnumerable<Topic>> GetAllTopics(bool includeUser, CancellationToken cancellationToken, TopicParameters requestParameters);
    Task<IEnumerable<Topic>> GetAllTopics(CancellationToken cancellationToken);
    Task<IEnumerable<Topic>> GetTopicsByUserId(string userId, CancellationToken cancellationToken, RequestParameters requestParameters);
    Task<IEnumerable<Topic>> GetArchivableTopics(int archiveAfterDays, CancellationToken cancellationToken); 
    Task<IEnumerable<Topic>> GetUnconfirmedTopics(CancellationToken cancellationToken, RequestParameters parameters, bool includeAuthor);
    Task DeleteTopicsByUserId(string userId);
}