using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using WireMess.Models.Entities;

namespace WireMess.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ConfigureBaseEntity();

            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(u => u.Email)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(u => u.PasswordHash)
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(u => u.PasswordSalt)
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);
            builder.Property(u => u.AvatarUrl)
                .HasMaxLength(500);
            builder.Property(u => u.IsOnline)
                .HasDefaultValue(false);
            builder.Property(u => u.LastActive)
                .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            builder.HasMany(u => u.Messages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.UserConversations)
                .WithOne(uc => uc.User)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
