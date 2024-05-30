using Common.DTOs.Topic.Request;
using Common.Exceptions;
using Common.Parameters;
using Services.Contracts.Contracts;
using Tests.Mocks;
using Xunit.Abstractions;

namespace Tests.ServiceTests;

public class TopicServiceTests : TestBase
{
    # region Setup
        
    private readonly ITestOutputHelper _testOutputHelper;
    private ITopicService _topicService = null!;
    private ServiceMock _serviceMock = null!;

    public TopicServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        
        MockSetup();
    }

    private void MockSetup()
    {
        _serviceMock = new ServiceMock();
        _topicService = _serviceMock.GetServiceManager().TopicService;
    }
    
    #endregion
    
    #region Tests
    
    [Fact]
    public async Task GetTopicById_WhenTopicExists_ShouldReturnTopic()
    {
        var topic = _serviceMock.RandomDataGenerator.GeneratedTopicData.First();
        
        var result = await _topicService.GetTopicById(
            topic.Id,
            false,
            false,
            CancellationToken.None
        );
        
        Assert.NotNull(result);
        Assert.Equal(topic.Id, result.Id);
        Assert.Equal(topic.Title, result.Title);
    }
    
    [Fact]
    public async Task GetTopicById_WhenTopicDoesNotExist_ShouldThrowException()
    {
        await Assert.ThrowsAsync<NotFound>(() => _topicService.GetTopicById(
            uint.MaxValue,
            false,
            false,
            CancellationToken.None
        ));
    }
    
    [Fact]
    public async Task GetTopics_WhenTopicsExist_ShouldReturnTopics()
    {
        var result = await _topicService.GetAllTopics(false,
            CancellationToken.None, new TopicParameters());
        
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task CreateTopic_WhenValid_ShouldCreateTopic()
    {
        var topicCreateModel = new TopicCreateModel
        (
            "Test",
            "TestContent",
            _serviceMock.RandomDataGenerator.GeneratedUserData.First(u=>
                u.Comments?.Count()
                >
                int.Parse(Configuration["ForumConfiguration:MinCommentsRequired"])
                ).Id
        );
        
        var user = _serviceMock.RandomDataGenerator.GeneratedUserData.First();

        _testOutputHelper.WriteLine(topicCreateModel.ToString());

        var result = await _topicService.CreateTopic(
            user.Id,
            topicCreateModel, 
            CancellationToken.None
            );

        Assert.NotNull(result);
        Assert.Equal(topicCreateModel.Title, result.Title);
        Assert.Equal(user.Id, result.AuthorId);
    }
    
    [Fact]
    public async Task UpdateTopic_WhenTopicDoesNotExist_ShouldThrowException()
    {
        var topicUpdateModel = new TopicUpdateModel
        (
            "Test",
            "TestContent",
            null
        );

        await Assert.ThrowsAsync<NotFound>(() =>
            _topicService.UpdateTopic(
                _serviceMock.RandomDataGenerator.GeneratedUserData.First().Id,
                uint.MaxValue,
                topicUpdateModel,
                CancellationToken.None
            )
        );
    }
    
    [Fact]
    public async Task UpdateTopic_WhenUserIsNotAuthor_ShouldThrowException()
    {
        var topicUpdateModel = new TopicUpdateModel
        (
            "Test",
            "TestContent",
            null
        );

        await Assert.ThrowsAsync<Forbidden>(() =>
        {
            var user = _serviceMock.RandomDataGenerator.GeneratedUserData.First();
            return _topicService.UpdateTopic(
                user.Id,
                _serviceMock.RandomDataGenerator.GeneratedTopicData.First(t => t.AuthorId != user.Id).Id,
                topicUpdateModel,
                CancellationToken.None
            );
        });
    }
    
    [Fact]
    public async Task UpdateTopic_WhenTopicExists_ShouldUpdateTopic()
    {
        var topic = _serviceMock.RandomDataGenerator.GeneratedTopicData.First();
        var topicUpdateModel = new TopicUpdateModel
        (
            "Test",
            "TestContent",
            null
        );

        var result = await _topicService.UpdateTopic(
            topic.AuthorId,
            topic.Id,
            topicUpdateModel,
            CancellationToken.None
        );
        
        _testOutputHelper.WriteLine(result.ToString());

        Assert.NotNull(result);
        Assert.Equal(topicUpdateModel.Title, result.Title);
        Assert.NotNull(result.BackgroundImageUrl);
    }
    
    [Fact]
    public async Task DeleteTopic_WhenTopicDoesNotExist_ShouldThrowException()
    {
        await Assert.ThrowsAsync<NotFound>(() =>
            _topicService.DeleteTopic(
                _serviceMock.RandomDataGenerator.GeneratedUserData.First().Id,
                uint.MaxValue,
                CancellationToken.None
            )
        );
    }
    
    [Fact]
    public async Task DeleteTopic_WhenUserIsNotAuthor_ShouldThrowException()
    {
        var user = _serviceMock.RandomDataGenerator.GeneratedUserData.First();
        await Assert.ThrowsAsync<Forbidden>(() =>
            _topicService.DeleteTopic(
                user.Id,
                _serviceMock.RandomDataGenerator.GeneratedTopicData.First(t => t.AuthorId != user.Id).Id,
                CancellationToken.None
            )
        );
    }
    
    [Fact]
    public async Task DeleteTopic_WhenTopicExists_ShouldDeleteTopic()
    {
        var topic = _serviceMock.RandomDataGenerator.GeneratedTopicData.First();
        
        await _topicService.DeleteTopic(
            topic.AuthorId,
            topic.Id,
            CancellationToken.None
        );
        
       await Assert.ThrowsAsync<NotFound>(() => _topicService.GetTopicById(
           topic.Id,
           false,
           false,
           CancellationToken.None
       ));
    }
    
    [Fact]
    public async Task GetCommentsByTopicId_WhenTopicExists_ShouldReturnComments()
    {
        var topic = _serviceMock.RandomDataGenerator.GeneratedTopicData.First(t=>t.CommentCount > 0);

        var result = await _topicService.GetCommentsByTopicId(
            topic.Id,
            false,
            CancellationToken.None,
            new RequestParameters()
        );

        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetCommentsByTopicId_WhenTopicDoesNotExist_ShouldThrowException()
    {
        await Assert.ThrowsAsync<NotFound>(() => 
            _topicService.GetCommentsByTopicId(
                uint.MaxValue,
                false,
                CancellationToken.None,
                new RequestParameters()
            ));
    }
    
    #endregion
}