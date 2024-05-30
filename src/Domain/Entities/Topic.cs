using Common.Enums;

namespace Domain.Entities;

// ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
public class Topic : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string BackgroundImageUrl { get; set; } = null!;

    public State State { get; set; } = State.Pending;
    public Status Status { get; set; } = Status.Active;
    public uint CommentCount { get; set; }

    public string AuthorId { get; set; } = null!;
    public virtual User Author { get; set; } = null!;
    public virtual IEnumerable<Comment>? Comments {get; set; }

    public override string ToString() =>
        $"Id: {Id}, Title: {Title}, Content: {Content}, BackgroundImageUrl: {BackgroundImageUrl}, State: {State}, Status: {Status}, CommentCount: {CommentCount}, AuthorId: {AuthorId}";
}