using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Email).HasMaxLength(80);
        builder.Property(u => u.ProfilePicUrl).HasMaxLength(300);
        builder.Property(u => u.RefreshToken).HasMaxLength(50);

        SeedData(builder);
    }
    
    private void SeedData(EntityTypeBuilder<User> builder)
    {
        
    }
}