using Repository.Contracts.Contracts;

namespace Repository.Contracts;

public interface IRepositoryManager
{
    IActionLogRepository ActionLogRepository { get; }
    IBanRepository BanRepository { get; }
    ICommentRepository CommentRepository { get; }
    ITopicRepository TopicRepository { get; }
    IUserRepository UserRepository { get; }

    Task SaveAsync(CancellationToken cancellationToken);
}