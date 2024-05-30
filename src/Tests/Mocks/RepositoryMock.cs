using Common.Parameters;
using Domain.Entities;
using Moq;
using Repository.Contracts;
using Repository.Contracts.Contracts;

namespace Tests.Mocks;

public class RepositoryMock
{
    private ITopicRepository? _topicMockInstance;
    private IBanRepository? _banMockInstance;
    private IUserRepository? _userMockInstance;
    private ICommentRepository? _commentMockInstance;
    
    public IRepositoryManager RepositoryMockInstance { get; }

    public readonly RandomDataGenerator RandomDataGenerator;
    
    public RepositoryMock()
    {
        var mockInstance = new Mock<IRepositoryManager>();
        
        RandomDataGenerator = new RandomDataGenerator();
        
        RandomDataGenerator.GenerateAllStaticBogusData();
        
        var topicMock = GetTopicRepository();
        var banMock = GetBanRepository();
        var userMock = GetUserRepository();
        var commentMock = GetCommentRepository();
        
        mockInstance.Setup(s => s.BanRepository).Returns(banMock);
        mockInstance.Setup(s => s.UserRepository).Returns(userMock);
        mockInstance.Setup(s => s.TopicRepository).Returns(topicMock);
        mockInstance.Setup(s => s.CommentRepository).Returns(commentMock);
        
        RepositoryMockInstance = mockInstance.Object;
    }

    #region Repository Mocks
    private ITopicRepository GetTopicRepository()
    {
        if (_topicMockInstance != null)
            return _topicMockInstance;

        var mockInstance = new Mock<ITopicRepository>();
        
        mockInstance.Setup(s => s.GetAllTopics(
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<TopicParameters>()))
            .Returns(Task.FromResult(RandomDataGenerator.GeneratedTopicData));
        
        mockInstance.Setup(s => s.GetTopicById(
                It.IsAny<uint>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns((uint topicId, bool _, bool _, CancellationToken _) =>
            {
                var topic = RandomDataGenerator.GeneratedTopicData.FirstOrDefault(x => x.Id == topicId);
                return Task.FromResult(topic);
            });
        
        mockInstance.Setup(s => s.GetTopicsByUserId(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<RequestParameters>()))
            .Returns((string userId, CancellationToken _, RequestParameters _) =>
            {
                var topics = RandomDataGenerator.GeneratedTopicData.Where(x => x.AuthorId == userId);
                return Task.FromResult(topics);
            });
        
        mockInstance.Setup(s => s.GetArchivableTopics(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Returns((int archiveAfterDays, CancellationToken _) =>
            {
                var topics = RandomDataGenerator.GeneratedTopicData.Where(x => x.CreationDate.AddDays(archiveAfterDays) < DateTime.Now);
                return Task.FromResult(topics);
            });
        
        mockInstance.Setup(s => s.Delete(It.IsAny<Topic>()))
            .Callback((Topic topic) =>
            {
                ((List<Topic>)RandomDataGenerator.GeneratedTopicData).Remove(topic);
            });
        
        _topicMockInstance = mockInstance.Object;
        return _topicMockInstance;
    }

    public IBanRepository GetBanRepository()
    {
        if (_banMockInstance != null)
            return _banMockInstance;
        
        var mockInstance = new Mock<IBanRepository>();
        
        mockInstance.Setup(s => s.GetBans(
                It.IsAny<RequestParameters>(),
                It.IsAny<CancellationToken>())
            )
            .Returns(Task.FromResult(RandomDataGenerator.GeneratedBanData));
        
        mockInstance.Setup(s => s.GetBanById(
                It.IsAny<uint>(), 
                It.IsAny<bool>(), 
                It.IsAny<CancellationToken>()))
            .Returns((uint banId, bool _, CancellationToken _) =>
            {
                var ban = RandomDataGenerator.GeneratedBanData.FirstOrDefault(x => x.Id == banId);
                return Task.FromResult(ban);
            });

        mockInstance.Setup(s => s.GetBanByUserId(
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<CancellationToken>()))
            .Returns((string userId, bool _, CancellationToken _) =>
            {
                var ban = RandomDataGenerator.GeneratedBanData.FirstOrDefault(x => x.UserId == userId);
                return Task.FromResult(ban);
            });
        
        mockInstance.Setup(s => s.GetExpiredBans(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(RandomDataGenerator.GeneratedBanData.Where(x => x.BanEndDate < DateTime.Now)));
        
        mockInstance.Setup(s => s.Delete(It.IsAny<Ban>()))
            .Callback((Ban ban) =>
            {
                ((List<Ban>)RandomDataGenerator.GeneratedBanData).Remove(ban);
            });
        
        _banMockInstance = mockInstance.Object;
        return _banMockInstance;
    }
    
    public IUserRepository GetUserRepository()
    {
        if (_userMockInstance != null)
            return _userMockInstance;
        
        var mockInstance = new Mock<IUserRepository>();
        
        mockInstance.Setup(s => s.GetUserById(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns((string userId, bool _, CancellationToken _) =>
            {
                var user = RandomDataGenerator.GeneratedUserData.FirstOrDefault(x => x.Id == userId);
                return Task.FromResult(user);
            });
        
        mockInstance.Setup(s => s.GetUserByEmail(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns((string email, bool _, CancellationToken _) =>
            {
                var user = RandomDataGenerator.GeneratedUserData.FirstOrDefault(x => x.Email == email);
                return Task.FromResult(user);
            });

        mockInstance.Setup(s => s.GetAllUsers(
                It.IsAny<CancellationToken>(),
                It.IsAny<RequestParameters>()))
            .Returns(Task.FromResult(RandomDataGenerator.GeneratedUserData));
        
        mockInstance.Setup(s => s.Delete(It.IsAny<User>()))
            .Callback((User user) =>
            {
                ((List<User>)RandomDataGenerator.GeneratedUserData).Remove(user);
            });
        
        _userMockInstance = mockInstance.Object;
        return _userMockInstance;
    }
    
    public ICommentRepository GetCommentRepository()
    {
        if (_commentMockInstance != null)
            return _commentMockInstance;
        
        var mockInstance = new Mock<ICommentRepository>();
        
        mockInstance.Setup(s => s.GetCommentById(
            It.IsAny<uint>(),
            It.IsAny<bool>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .Returns((uint commentId, bool _, bool _, CancellationToken _) =>
            {
                var comment = RandomDataGenerator.GeneratedCommentData.FirstOrDefault(x => x.Id == commentId);
                return Task.FromResult(comment);
            });
        
        mockInstance.Setup(s => s.GetCommentsByTopicId(
            It.IsAny<uint>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestParameters>()))
            .Returns((uint topicId, bool _, CancellationToken _, RequestParameters _) =>
            {
                var comments = RandomDataGenerator.GeneratedCommentData.Where(x => x.TopicId == topicId);
                return Task.FromResult(comments);
            });
        
        mockInstance.Setup(s => s.GetCommentsByUserId(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestParameters>()))
            .Returns((string userId, CancellationToken _, RequestParameters _) =>
            {
                var comments = RandomDataGenerator.GeneratedCommentData.Where(x => x.AuthorId == userId);
                return Task.FromResult(comments);
            });
        
        mockInstance.Setup(s => s.GetCommentsCountByTopicId(
            It.IsAny<uint>(),
            It.IsAny<CancellationToken>()))
            .Returns((uint topicId, CancellationToken _) =>
            {
                var comments = RandomDataGenerator.GeneratedCommentData.Count(x => x.TopicId == topicId);
                return Task.FromResult(comments);
            });
        
        mockInstance.Setup(s => s.GetCommentsCountByUserId(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .Returns((string userId, CancellationToken _) =>
            {
                var comments = RandomDataGenerator.GeneratedCommentData.Count(x => x.AuthorId == userId);
                return Task.FromResult(comments);
            });
        
        mockInstance.Setup(s => s.Delete(It.IsAny<Comment>()))
            .Callback((Comment comment) =>
            {
                ((List<Comment>)RandomDataGenerator.GeneratedCommentData).Remove(comment);
            });
        
        _commentMockInstance = mockInstance.Object;
        return _commentMockInstance;
    }
    #endregion
}