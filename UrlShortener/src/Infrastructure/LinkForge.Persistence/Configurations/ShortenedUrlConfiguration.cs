namespace LinkForge.Persistence.Configurations;

public class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
{
    public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.ShortCode).IsUnique();
        
        builder.Property(x => x.OriginalUrl)
            .IsRequired()
            .HasMaxLength(2048);
            
        builder.Property(x => x.ShortCode)
            .IsRequired()
            .HasMaxLength(15);
            
        builder.HasIndex(x => x.CustomAlias)
            .IsUnique()
            .HasFilter("\"CustomAlias\" IS NOT NULL");
            
        // Soft delete filter
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
