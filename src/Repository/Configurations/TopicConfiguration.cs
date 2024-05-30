using Common.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder
            .HasOne(t => t.Author)
            .WithMany(a => a.Topics)
            .HasForeignKey(t => t.AuthorId)
            .IsRequired(false);

        builder.Property(t => t.Title).HasMaxLength(80);
        builder.Property(t => t.BackgroundImageUrl).HasMaxLength(300).IsRequired(false);
        builder.Property(t => t.Content).HasMaxLength(2000);

        builder.Property(t => t.Status)
            .HasConversion(s => s == Status.Active,
                s => s
                    ? Status.Active
                    : Status.Inactive);

        builder.HasIndex(t => t.Title);
        builder.HasIndex(t => t.CommentCount);
    
        SeedData(builder);
    }
    
    private void SeedData(EntityTypeBuilder<Topic> builder)
    {
        
    }
}