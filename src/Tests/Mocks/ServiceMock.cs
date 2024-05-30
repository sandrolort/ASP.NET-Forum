using Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using Services;
using Services.Contracts;
using Services.Contracts.Contracts;
using Services.Services;

namespace Tests.Mocks;

public class ServiceMock
{
    private readonly ILoggerService _logger;
    private ServiceManager? _serviceManager;
    
    private BanService? _banService;
    private UserService? _userService;
    private CommentService? _commentService;
    private TopicService? _topicService;

    private readonly IConfiguration? _configuration = null!;

    private RepositoryMock RepositoryMock { get; } = new();
    public RandomDataGenerator RandomDataGenerator => RepositoryMock.RandomDataGenerator;

    public ServiceMock()
    {
        _logger = new ConsoleLogger();

        var config = new Dictionary<string, string>
        {
            { "ForumConfiguration:MinCommentsRequired", "5"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(config)
            .Build();
    }
    
    public ServiceManager GetServiceManager()
    {
        if (_serviceManager != null)
            return _serviceManager;
        
        var serviceMock = new Mock<IServiceManager>();
        
        serviceMock.Setup(s => s.CommentService).Returns(GetCommentService());
        serviceMock.Setup(s => s.UserService).Returns(GetUserService());
        serviceMock.Setup(s => s.BanService).Returns(GetBanService());
        serviceMock.Setup(s => s.TopicService).Returns(GetTopicService());
        
        _serviceManager = new ServiceManager(RepositoryMock.RepositoryMockInstance, _logger, _configuration, UserManagerMock.GetMockUserManager());
        return _serviceManager;
    }
    
    #region Service Mocks
        public CommentService GetCommentService()
        {
            if (_commentService != null)
                return _commentService;
            
            var commentMock = new CommentService(
                RepositoryMock.RepositoryMockInstance,
                _logger
            );
            
            _commentService = commentMock;
            return commentMock;
        }
        
        public UserService GetUserService()
        {
            if (_userService != null)
                return _userService;
            
            var userMock = new UserService(
                RepositoryMock.RepositoryMockInstance,
                _logger
            );
            
            _userService = userMock;
            return userMock;
        }
        
        public BanService GetBanService()
        {
            if (_banService != null)
                return _banService;
            
            var banMock = new BanService(
                RepositoryMock.RepositoryMockInstance,
                _logger,
                UserManagerMock.Instance().Object
            );
            
            _banService = banMock;
            return banMock;
        }
        
        public TopicService GetTopicService()
        {
            if (_topicService != null)
                return _topicService;
            
            var topicMock = new TopicService(
                RepositoryMock.RepositoryMockInstance,
                _logger,
                _configuration!
            );
            
            _topicService = topicMock;
            return topicMock;
        }
    #endregion
}