using Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts;
using Repository.Contracts.Contracts;
using Repository.Repositories;

namespace Repository;

/// <summary>
/// Unit of work. Done using Lazy evaluation.
/// </summary>
/// <remarks>
/// This can also be implemented by writing an Unit of work per combination required.
/// I'd rather have it like this.
/// </remarks>
public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    
    private Lazy<ActionLogRepository> _actionLogRepository;
    private Lazy<BanRepository> _banRepository;
    private Lazy<CommentRepository> _commentRepository;
    private Lazy<TopicRepository> _topicRepository;
    private Lazy<UserRepository> _userRepository;
    
    public IActionLogRepository ActionLogRepository => _actionLogRepository.Value;
    public IBanRepository BanRepository => _banRepository.Value;
    public ICommentRepository CommentRepository => _commentRepository.Value;
    public ITopicRepository TopicRepository => _topicRepository.Value;
    public IUserRepository UserRepository => _userRepository.Value;
    
    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        
        _actionLogRepository = new Lazy<ActionLogRepository>(() => new ActionLogRepository(_repositoryContext));
        _banRepository = new Lazy<BanRepository>(() => new BanRepository(_repositoryContext));
        _commentRepository = new Lazy<CommentRepository>(() => new CommentRepository(_repositoryContext));
        _topicRepository = new Lazy<TopicRepository>(() => new TopicRepository(_repositoryContext));
        _userRepository = new Lazy<UserRepository>(() => new UserRepository(_repositoryContext));
    }
    
    public Task SaveAsync(CancellationToken cancellationToken)
    {
        //setting the modified date for all entities properly
        var modifiedEntries = _repositoryContext.ChangeTracker.Entries()
            .Where(x=>x.State == EntityState.Modified);
        foreach (var entry in modifiedEntries)
        {
            if (entry.Entity is not BaseEntity) continue;
            entry.CurrentValues["ModifiedDate"] = DateTime.Now;
        }
        
        //Auditing the changes
        RepositoryAudit.Audit(_repositoryContext);
        
        return _repositoryContext.SaveChangesAsync(cancellationToken);
    }
}