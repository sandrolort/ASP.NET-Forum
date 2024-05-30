using Domain.Entities;

namespace Repository.Contracts.Contracts;

public interface IActionLogRepository
{
    Task<IEnumerable<ActionLog>> GetAllActionLogs(CancellationToken cancellationToken);
    Task<ActionLog?> GetActionLogById(uint actionLogId, CancellationToken cancellationToken);
    Task<IEnumerable<ActionLog>> GetUserLogs(string userId, CancellationToken cancellationToken);
    Task<IEnumerable<ActionLog>> GetBanLogs(uint banId, CancellationToken cancellationToken);
    Task<IEnumerable<ActionLog>> GetCommentLogs(uint commentId, CancellationToken cancellationToken);
    Task<IEnumerable<ActionLog>> GetTopicLogs(uint topicId, CancellationToken cancellationToken);
}