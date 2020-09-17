using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Infra.Mappings
{
    internal class CommentMapConfig : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("comments");

            builder.Property(c => c.Id).HasColumnName("id");

            builder.Property(c => c.Text)
                .IsRequired()
                .HasMaxLength(1500)
                .HasColumnName("text");

            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.ReviewId).HasColumnName("review_id");
        }
    }
}
