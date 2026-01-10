using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WireMess.Models.Entities;

namespace WireMess.Data.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.ConfigureBaseEntity();

            builder.ToTable("Conversations");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ConversationName)
                .HasMaxLength(100)
                .IsRequired(false);
            builder.Property(c => c.LastMessageAt)
                .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            builder.Property(c => c.AvatarUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.UserConversation)
                .WithOne(uc => uc.Conversation)
                .HasForeignKey(uc => uc.ConversationId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
            builder.HasOne(c => c.ConversationType)
                .WithMany(ct => ct.Conversations)
                .HasForeignKey(c => c.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
