using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WireMess.Models.Entities;

namespace WireMess.Data.Configurations
{
    public class ConversationTypeConfiguration : IEntityTypeConfiguration<ConversationType>
    {
        public void Configure(EntityTypeBuilder<ConversationType> builder)
        {
            builder.ConfigureBaseEntity();

            builder.ToTable("ConversationTypes");
            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.TypeName)
                .HasMaxLength(20)
                .IsRequired();
            builder.Property(ct => ct.Description)
              .HasMaxLength(100);
        }
    }
}
