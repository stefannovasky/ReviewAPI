using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Infra.Mappings
{
    class ReviewMapConfig : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("reviews");

            builder.Property(p => p.Id).HasColumnName("id");

            builder.Property(p => p.Title)
                .HasColumnName("title")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.Text)
                .HasColumnName("text")
                .HasMaxLength(1500)
                .IsRequired();

            builder.Property(p => p.Stars)
                .HasColumnName("stars")
                .IsRequired();

            builder.Property(p => p.CreatorId)
                .HasColumnName("creator_id")
                .HasMaxLength(36)
                .IsRequired();

            builder.HasOne<User>(p => p.Creator)
                .WithMany(u => u.Reviews)
                .HasForeignKey(p => p.CreatorId);
            builder.HasOne<ReviewImage>(a => a.Image)
               .WithOne(b => b.Review)
               .HasForeignKey<ReviewImage>(b => b.ReviewId);
        }
    }
}
