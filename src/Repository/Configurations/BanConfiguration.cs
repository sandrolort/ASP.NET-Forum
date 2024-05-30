using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class BanConfiguration : IEntityTypeConfiguration<Ban>
{
    public void Configure(EntityTypeBuilder<Ban> builder)
    {
        builder
            .HasOne(b => b.BannedUser)
            .WithOne(u => u.BanInfo)
            .HasForeignKey<Ban>(b => b.UserId);

        builder.Property(b => b.Reason).HasMaxLength(100);
        
        builder.HasIndex(b => b.BanEndDate);
        
        SeedData(builder);
    }
    
    private void SeedData(EntityTypeBuilder<Ban> builder)
    {
        
    }
}