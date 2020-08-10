using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Infra.Mappings
{
    public class ReviewImageMapConfig : IEntityTypeConfiguration<ReviewImage>
    {
        public void Configure(EntityTypeBuilder<ReviewImage> builder)
        {
            builder.ToTable("review_images");

            builder.Property(p => p.Id).HasColumnName("id");

            builder.Property(p => p.ReviewId)
                .HasColumnName("review_id")
                .HasMaxLength(36)
                .HasDefaultValue(null);

            builder.Property(p => p.FileName)
                .HasColumnName("filename")
                .HasMaxLength(41)
                .IsRequired();

            builder.Property(p => p.FilePath)
                .HasColumnName("filepath")
                .HasMaxLength(300)
                .IsRequired();

            builder.HasOne<Review>(a => a.Review)
                .WithOne(b => b.Image);
        }
    }
}
