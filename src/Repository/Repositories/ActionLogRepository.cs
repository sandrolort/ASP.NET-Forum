using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts.Contracts;

namespace Repository.Repositories;

public class ActionLogRepository : RepositoryBase<ActionLog>, IActionLogRepository
{
    public ActionLogRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }
    
    public async Task<IEnumerable<ActionLog>> GetAllActionLogs(CancellationToken cancellationToken) =>
        await RepositoryContext.ActionLogs
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public Task<ActionLog?> GetActionLogById(uint actionLogId, CancellationToken cancellationToken) =>
        RepositoryContext.ActionLogs
            .Where(a => a.Id == actionLogId)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<ActionLog>> GetUserLogs(string userId, CancellationToken cancellationToken) =>
        await RepositoryContext.ActionLogs
            .Where(a => a.ItemType == nameof(User) && a.ItemId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ActionLog>> GetBanLogs(uint banId, CancellationToken cancellationToken) =>
        await RepositoryContext.ActionLogs
            .Where(a => a.ItemType == nameof(Ban) && a.ItemId == banId.ToString())
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ActionLog>> GetCommentLogs(uint commentId, CancellationToken cancellationToken) =>
        await RepositoryContext.ActionLogs
            .Where(a => a.ItemType == nameof(Comment) && a.ItemId == commentId.ToString())
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ActionLog>> GetTopicLogs(uint topicId, CancellationToken cancellationToken) =>
        await RepositoryContext.ActionLogs
            .Where(a => a.ItemType == nameof(Topic) && a.ItemId == topicId.ToString())
            .AsNoTracking()
            .ToListAsync(cancellationToken);
}