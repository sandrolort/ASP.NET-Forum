using System.ComponentModel.DataAnnotations;

namespace Common.DTOs.Topic.Request;

public record TopicCreateModel(
    [MaxLength(80)]
    string? Title, 
    [MaxLength(2000)]
    string? Content, 
    [MaxLength(300)]
    string? BackgroundImageUrl
);