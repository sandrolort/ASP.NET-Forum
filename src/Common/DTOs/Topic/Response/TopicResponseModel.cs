using Common.Enums;

namespace Common.DTOs.Topic.Response;

public record TopicResponseModel(
    uint Id,
    string Title, 
    string Content, 
    string BackgroundImageUrl, 
    string AuthorId, 
    string? AuthorUserName,
    string? AuthorProfilePicUrl,
    int? CommentCount,
    Status Status, 
    State State,
    DateTime CreationDate,
    DateTime ModifiedDate
);