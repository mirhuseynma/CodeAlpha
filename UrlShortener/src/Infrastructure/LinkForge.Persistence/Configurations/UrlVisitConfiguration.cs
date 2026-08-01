namespace LinkForge.Persistence.Configurations;

public class UrlVisitConfiguration : IEntityTypeConfiguration<UrlVisit>
{
    public void Configure(EntityTypeBuilder<UrlVisit> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.IpAddress).HasMaxLength(45);
        builder.Property(x => x.UserAgent).HasMaxLength(1000);
        builder.Property(x => x.Referer).HasMaxLength(2048);
        builder.Property(x => x.Country).HasMaxLength(100);

        builder.HasOne(x => x.ShortenedUrl)
            .WithMany(x => x.Visits)
            .HasForeignKey(x => x.ShortenedUrlId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
