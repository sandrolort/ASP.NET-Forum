using Common.DTOs.Comment.Request;
using Common.Exceptions;
using Services.Contracts.Contracts;
using Tests.Mocks;
using Xunit.Abstractions;

namespace Tests.ServiceTests;

public class CommentServiceTests : TestBase
{
    #region Setup

    private readonly ITestOutputHelper _testOutputHelper;
    private ICommentService _commentService = null!;
    private ServiceMock _serviceMock = null!;

    public CommentServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        
        MockSetup();
    }

    private void MockSetup()
    {
        _serviceMock = new ServiceMock();
        _commentService = _serviceMock.GetServiceManager().CommentService;
    }

    #endregion

    #region Tests
    
    [Fact]
    public async Task CreateComment_WhenValid_ShouldCreateComment()
    {
        var commentCreateModel = new CommentCreateModel
        (
            "Test",
            _serviceMock.RandomDataGenerator.GeneratedTopicData.First().Id
        );

        _testOutputHelper.WriteLine(commentCreateModel.ToString());

        var result = await _commentService.CreateComment(
            _serviceMock.RandomDataGenerator.GeneratedUserData.First().Id,
            commentCreateModel, 
            CancellationToken.None
            );

        Assert.NotNull(result);
        Assert.Equal(commentCreateModel.Content, result.Content);
        Assert.Equal(commentCreateModel.TopicId, result.TopicId);
    }
    
    [Fact]
    public async Task UpdateComment_WhenCommentDoesNotExist_ShouldThrowException()
    {
        var commentUpdateModel = new CommentUpdateModel
        (
            "Test"
        );

        await Assert.ThrowsAsync<NotFound>(() => 
            _commentService.UpdateComment(
                "nonexistentId",
                uint.MaxValue,
                commentUpdateModel, 
                CancellationToken.None
            ));
    }
    
    [Fact]
    public async Task UpdateComment_WhenCommentExists_ShouldUpdateComment()
    {
        var comment = _serviceMock.RandomDataGenerator.GeneratedCommentData.First();
        
        _testOutputHelper.WriteLine(comment.ToString());
        
        var commentUpdateModel = new CommentUpdateModel
        (
            "Test"
        );

        _testOutputHelper.WriteLine(commentUpdateModel.ToString());

        var result = await _commentService.UpdateComment(
            comment.AuthorId,
            comment.Id,
            commentUpdateModel, 
            CancellationToken.None
        );

        Assert.NotNull(result);
        Assert.Equal(commentUpdateModel.Content, result.Content);
    }
    
    [Fact]
    public async Task DeleteComment_WhenCommentExists_ShouldDeleteComment()
    {
        var comment = _serviceMock.RandomDataGenerator.GeneratedCommentData.First();

        await _commentService.DeleteComment(
            comment.AuthorId,
            comment.Id, 
            CancellationToken.None
        );
        
        await Assert.ThrowsAsync<NotFound>(()=>_commentService.GetCommentById(
            comment.Id,
            false,
            false,
            CancellationToken.None)
        );
    }
    
    [Fact]
    public async Task DeleteComment_WhenCommentDoesNotExist_ShouldThrowException()
    {
        await Assert.ThrowsAsync<NotFound>(() => 
            _commentService.DeleteComment(
                "nonexistentId",
                uint.MaxValue, 
                CancellationToken.None
            ));
    }
    
    [Fact]
    public async Task GetCommentById_WhenCommentExists_ShouldReturnComment()
    {
        var comment = _serviceMock.RandomDataGenerator.GeneratedCommentData.First();

        _testOutputHelper.WriteLine(comment.ToString());

        var result = await _commentService.GetCommentById(
            comment.Id,
            false,
            false,
            CancellationToken.None
        );

        Assert.NotNull(result);
        Assert.Equal(comment.Id, result.Id);
    }
    
    [Fact]
    public async Task GetCommentById_WhenCommentDoesNotExist_ShouldThrowException()
    {
        await Assert.ThrowsAsync<NotFound>(() => 
            _commentService.GetCommentById(
                uint.MaxValue,
                false,
                false,
                CancellationToken.None
            ));
    }

    #endregion
}