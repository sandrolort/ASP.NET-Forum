using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public record TopicCreateModelWithImage(
    [Required]
    [MaxLength(80)]
    string? Title, 
    [Required]
    [MaxLength(2000)]
    string? Content, 
    IFormFile? Image);