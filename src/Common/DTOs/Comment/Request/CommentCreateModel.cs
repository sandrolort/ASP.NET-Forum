using System.ComponentModel.DataAnnotations;

namespace Common.DTOs.Comment.Request;

public record CommentCreateModel(
    [MaxLength(2000)]
    string? Content, 
    [Required]
    uint? TopicId
);