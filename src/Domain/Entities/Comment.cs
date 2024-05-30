namespace Domain.Entities;

// ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
public class Comment : BaseEntity
{
    public string Content { get; set; } = null!;

    public uint TopicId { get; set; }
    public virtual Topic Topic { get; set; } = null!;
    
    public string AuthorId { get; set; } = null!;
    public virtual User Author { get; set; } = null!;

    public override string ToString() => $"Id: {Id}, Content: {Content}, TopicId: {TopicId}, AuthorId: {AuthorId}";
}