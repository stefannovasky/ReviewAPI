using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Infra.Mappings
{
    internal class UserMapConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.Property(p => p.Id).HasColumnName("id");

            builder.Property(p => p.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.Password)
                .HasColumnName("password_hash")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(p => p.ConfirmationCode)
                .HasColumnName("confirmation_code")
                .HasMaxLength(8);
            
            builder.Property(p => p.Confirmed)
                .HasColumnName("confirmed")
                .HasDefaultValue(false);

            builder.Property(p => p.ResetPasswordCode)
                .HasColumnName("reset_password_code")
                .HasMaxLength(8);
        }
    }
}
