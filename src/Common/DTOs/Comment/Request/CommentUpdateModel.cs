using System.ComponentModel.DataAnnotations;

namespace Common.DTOs.Comment.Request;

public record CommentUpdateModel(
    [MaxLength(2000)]
    string? Content
);