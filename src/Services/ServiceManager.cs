using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Repository.Contracts;
using Services.Contracts;
using Services.Contracts.Contracts;
using Services.Services;

namespace Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<BanService> _banService;
    private readonly Lazy<CommentService> _commentService;
    private readonly Lazy<TopicService> _topicService;
    private readonly Lazy<UserService> _userService;
    private readonly Lazy<ImageService> _imageService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerService logger, IConfiguration configuration, UserManager<User> userManager)
    {
        _banService = new Lazy<BanService>(() => new BanService(repositoryManager, logger, userManager));
        _commentService = new Lazy<CommentService>(() => new CommentService(repositoryManager, logger));
        _topicService = new Lazy<TopicService>(() => new TopicService(repositoryManager, logger, configuration));
        _userService = new Lazy<UserService>(() => new UserService(repositoryManager, logger));
        _imageService = new Lazy<ImageService>(() => new ImageService(configuration, logger));
    }
    
    public IBanService BanService => _banService.Value;
    public ICommentService CommentService => _commentService.Value;
    public ITopicService TopicService => _topicService.Value;
    public IUserService UserService => _userService.Value;
    public IImageService ImageService => _imageService.Value;
}