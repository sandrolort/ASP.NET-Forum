using Common.DTOs;
using Common.DTOs.Comment.Request;
using Common.DTOs.Comment.Response;

namespace Services.Contracts.Contracts;

public interface ICommentService
{
    Task<CommentResponseModel> GetCommentById(uint commentId, bool trackChanges, bool includeUser, CancellationToken cancellationToken);
    
    Task<CommentResponseModel> CreateComment(string userId, CommentCreateModel commentCreateModel, CancellationToken cancellationToken);
    Task<CommentResponseModel> UpdateComment(string userId, uint commentId, CommentUpdateModel commentUpdateModel, CancellationToken cancellationToken);
    Task DeleteComment(string userId, uint commentId, CancellationToken cancellationToken);
    
    Task<IEnumerable<ActionLogResponse>> GetCommentLogs(uint commentId, CancellationToken cancellationToken);
    
    Task ReevaluateCommentCount(CancellationToken cancellationToken);
}