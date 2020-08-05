using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Infra.Mappings
{
    public class ProfileImageMapConfig : IEntityTypeConfiguration<ProfileImage>
    {
        public void Configure(EntityTypeBuilder<ProfileImage> builder)
        {
            builder.ToTable("profile_images");

            builder.Property(p => p.Id).HasColumnName("id");

            builder.Property(p => p.UserId)
                .HasColumnName("user_id")
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

            builder.HasOne<User>(a => a.User)
                .WithOne(b => b.ProfileImage);
        }
    }
}
