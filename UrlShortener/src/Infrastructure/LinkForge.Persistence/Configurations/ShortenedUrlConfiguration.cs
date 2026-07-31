using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkForge.Persistence.Configurations;

public class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
{
    public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.ShortCode)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.HasIndex(x => x.ShortCode).IsUnique();
        
        builder.Property(x => x.CustomAlias)
            .HasMaxLength(50);
            
        builder.HasIndex(x => x.CustomAlias).IsUnique();

        builder.HasOne(x => x.AppUser)
            .WithMany(u => u.ShortenedUrls)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

