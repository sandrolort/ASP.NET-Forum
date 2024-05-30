using Services.Contracts.Contracts;

namespace Services.Contracts;

public interface IServiceManager
{
    IBanService BanService { get; }
    ICommentService CommentService { get; }
    ITopicService TopicService { get; }
    IUserService UserService { get; }
    IImageService ImageService { get; }
}