using Common.DTOs.Comment.Response;

namespace Common.DTOs.Topic.Response;

public record TopicCommentResponseModel (TopicResponseModel Topic, IEnumerable<CommentResponseModel> Comments);