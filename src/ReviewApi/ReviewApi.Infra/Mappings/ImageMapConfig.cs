using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Infra.Mappings
{
    public class ImageMapConfig : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("images");

            builder.Property(p => p.Id).HasColumnName("id");

            builder.Property(p => p.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(36)
                .HasDefaultValue(null);

            builder.Property(p => p.FileName)
                .HasColumnName("filename")
                .IsFixedLength()
                .HasMaxLength(36)
                .IsRequired();

            builder.Property(p => p.FilePath)
                .HasColumnName("filepath")
                .HasMaxLength(300)
                .IsRequired();
        }
    }
}
