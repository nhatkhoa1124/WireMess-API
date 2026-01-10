using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WireMess.Models.Entities;

namespace WireMess.Data.Configurations
{
    public class UserConversationConfiguration : IEntityTypeConfiguration<UserConversation>
    {
        public void Configure(EntityTypeBuilder<UserConversation> builder)
        {
            builder.ConfigureBaseEntity();
            
            builder.ToTable("UserConversations");
            builder.HasKey(uc => new {uc.UserId, uc.ConversationId});
        }
    }
}
