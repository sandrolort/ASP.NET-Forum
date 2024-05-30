namespace Common.DTOs.Comment.Response;

public record CommentResponseModel(
    uint Id,
    string AuthorId, 
    uint TopicId, 
    string Content,
    string? AuthorUserName,
    string? AuthorProfilePicUrl,
    bool? AuthorIsAdmin
);